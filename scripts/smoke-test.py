#!/usr/bin/env python3
"""Exercise local flows against a disposable SQLite database; Python stdlib only."""
from html.parser import HTMLParser
import http.cookiejar
import os
from pathlib import Path
import socket
import sqlite3
import subprocess
import tempfile
import time
import urllib.error
import urllib.parse
import urllib.request

ROOT = Path(__file__).resolve().parents[1]


class Inputs(HTMLParser):
    def __init__(self, text):
        super().__init__()
        self.fields = {}
        self.feed(text)

    def handle_starttag(self, tag, attrs):
        attrs = dict(attrs)
        if tag == 'input' and attrs.get('name'):
            self.fields[attrs['name']] = attrs.get('value', '')


class Client:
    def __init__(self, base):
        self.base = base
        self.opener = urllib.request.build_opener(
            urllib.request.HTTPCookieProcessor(http.cookiejar.CookieJar()))

    def request(self, path, data=None, expected=200, body=None, content_type=None):
        url = self.base + path if path.startswith('/') else path
        if data is not None:
            body = urllib.parse.urlencode(data).encode()
            content_type = 'application/x-www-form-urlencoded'
        request = urllib.request.Request(url, data=body)
        if content_type:
            request.add_header('Content-Type', content_type)
        try:
            response = self.opener.open(request, timeout=15)
        except urllib.error.HTTPError as error:
            response = error
        text = response.read().decode(errors='replace')
        assert response.code == expected, f'{path}: expected {expected}, got {response.code}\n{text[:800]}'
        return text

    def post(self, path, data, token_page=None, expected=200):
        page = self.request(token_page or path)
        fields = Inputs(page).fields
        data = dict(data, __RequestVerificationToken=fields['__RequestVerificationToken'])
        return self.request(path, data, expected)

    def login(self, email, password):
        self.post('/Identity/Account/Login', {'Input.Email': email, 'Input.Password': password})


def register(client, email):
    client.post('/Identity/Account/Register', {
        'Input.Email': email, 'Input.Password': 'SmokeTest7!',
        'Input.ConfirmPassword': 'SmokeTest7!', 'Input.FullName': 'Smoke Customer',
        'Input.PhoneNumber': '01700000001'})
    assert 'id="logoutForm"' in client.request('/'), 'Registration did not sign in'


def run(base, database, mail_dir):
    with sqlite3.connect(database) as db:
        initial_category_count = db.execute("SELECT COUNT(*) FROM Category").fetchone()[0]
        assert initial_category_count == 7, "Demo categories were not seeded"
        assert db.execute("SELECT COUNT(*) FROM Product").fetchone()[0] == 16, "Demo products were not seeded"
    guest = Client(base)
    for path in ['/', '/Home/AboutUs', '/Home/ContactUs', '/Home/Privacy',
                 '/Home/Details/1', '/Identity/Account/Login', '/Identity/Account/Register',
                 '/Identity/Account/ForgotPassword', '/Identity/Account/ResendEmailConfirmation',
                 '/Identity/Account/LoginWithRecoveryCode', '/Identity/Account/LoginWith2fa?rememberMe=false']:
        guest.request(path)
    guest.request('/Home/Details/999999', expected=404)
    for path in ['/Order/IndexUser', '/WishList/Delete/1', '/Order/Index', '/WebSiteDetail/Index']:
        assert 'Log in' in guest.request(path), f'{path} should require login'
    guest.request('/Home/Details/1', data={'Product.TempQty': 1}, expected=400)
    print('PASS public pages, missing products, login guards and CSRF')

    customer = Client(base)
    register(customer, 'smoke@example.test')
    assert list(mail_dir.glob('*.html')), 'Registration email was not saved locally'
    customer.request('/WishList/Add/1')
    customer.request('/WishList/Index')
    customer.post('/Home/Details/1', {'Product.TempQty': 2})
    customer.request('/Cart/Index')
    with sqlite3.connect(database) as db:
        wishlist_id = db.execute('SELECT Id FROM WishListDetail').fetchone()[0]
    customer.request(f'/WishList/Delete/{wishlist_id}')
    assert 'Tomatoes' in customer.request('/Cart/Index'), 'Wishlist deletion corrupted cart'
    customer.post('/Home/Details/1', {'Product.TempQty': 0}, expected=400)
    customer.post('/Cart/UpdateCart', {'[0].Id': 1, '[0].TempQty': 3}, token_page='/Cart/Index')
    customer.request('/Cart/Summary')
    customer.post('/Cart/Summary', {
        'ApplicationUser.FullName': 'Smoke Customer', 'ApplicationUser.Email': 'smoke@example.test',
        'ApplicationUser.PhoneNumber': '01700000001', 'ApplicationUser.StreetAddress': 'Test Street',
        'ApplicationUser.City': 'Dhaka', 'ApplicationUser.PostalCode': '1200',
        'ProductList[0].Id': 1, 'ProductList[0].Price': 0.01, 'ProductList[0].TempQty': 99})
    with sqlite3.connect(database) as db:
        order_id, total = db.execute('SELECT Id, FinalOrderTotal FROM OrderHeader').fetchone()
        assert total == 300, f'Checkout trusted posted prices: {total}'
        assert db.execute('SELECT COUNT(*) FROM Cart').fetchone()[0] == 0
    customer.request('/Order/IndexUser')
    customer.request(f'/Order/DetailsUser/{order_id}')
    customer.request('/Order/DetailsUser/999999', expected=404)
    print('PASS registration, local email, wishlist, cart and checkout with tampered prices')

    other = Client(base)
    register(other, 'other@example.test')
    other.request(f'/Order/DetailsUser/{order_id}', expected=404)
    other.request(f'/Cart/OrderConfirmation/{order_id}', expected=404)
    assert 'access denied' in customer.request('/Order/Index').lower()
    customer.request('/Cart/Remove/999999')
    customer.request('/WishList/Add/999999', expected=404)
    print('PASS order ownership, customer permissions and stale IDs')

    admin = Client(base)
    admin.login('admin@mudi.local', 'MudiLocal7!')
    for path in ['/', '/Category', '/Category/Create', '/Category/Edit/1', '/Category/Delete/1',
                 '/Product', '/Product/Upsert', '/Product/Upsert/1', '/Product/Delete/1',
                 '/Order', f'/Order/Details/{order_id}', '/WebSiteDetail',
                 '/WebSiteDetail/EditAboutUs/1', '/WebSiteDetail/EditContactUs/1',
                 '/Identity/Account/Manage', '/Identity/Account/Manage/Email',
                 '/Identity/Account/Manage/ChangePassword', '/Identity/Account/Manage/ExternalLogins',
                 '/Identity/Account/Manage/TwoFactorAuthentication', '/Identity/Account/Manage/PersonalData']:
        admin.request(path)
    for path in ['/Category/Edit/999999', '/Product/Upsert/999999', '/Order/Details/999999',
                 '/WebSiteDetail/EditAboutUs/999999', '/WebSiteDetail/EditContactUs/999999']:
        admin.request(path, expected=404)
    for action in ['StartProcessing', 'ShipOrder', 'CompleteOrder']:
        admin.post('/Order/' + action, {'OrderHeader.Id': order_id}, token_page=f'/Order/Details/{order_id}')
    admin.post('/Order/CompleteOrder', {'OrderHeader.Id': order_id},
               token_page=f'/Order/Details/{order_id}', expected=400)
    with sqlite3.connect(database) as db:
        assert db.execute('SELECT Stock FROM Product WHERE Id=1').fetchone()[0] == 47
    admin.post('/Order/StartProcessing', {'OrderHeader.Id': order_id},
               token_page=f'/Order/Details/{order_id}', expected=400)
    admin.post('/Category/Create', {'Name': 'Smoke category', 'DisplayOrder': 2,
               'CategoryDescription': 'Temporary category'})
    with sqlite3.connect(database) as db:
        category_id = db.execute("SELECT Id FROM Category WHERE Name='Smoke category'").fetchone()[0]
    admin.post(f'/Category/Edit/{category_id}', {'Id': category_id, 'Name': 'Edited category',
               'DisplayOrder': 3, 'CategoryDescription': 'Updated category'})
    admin.post('/Category/DeletePost/1', {'Id': 1}, token_page='/Category/Delete/1')
    admin.post(f'/Category/DeletePost/{category_id}', {'Id': category_id}, token_page=f'/Category/Delete/{category_id}')
    admin.post('/WebSiteDetail/EditAboutUsPost', {'WebSiteDetailId': 1, 'AboutUs': 'Updated story'},
               token_page='/WebSiteDetail/EditAboutUs/1')
    admin.post('/WebSiteDetail/EditContactUsPost', {'WebSiteDetailId': 1, 'ContactUs': 'Updated contacts'},
               token_page='/WebSiteDetail/EditContactUs/1')
    with sqlite3.connect(database) as db:
        assert db.execute('SELECT COUNT(*) FROM Category').fetchone()[0] == initial_category_count
        assert db.execute('SELECT AboutUs, ContactUs FROM WebSiteDetail').fetchone() == ('Updated story', 'Updated contacts')
    print('PASS admin pages, category changes, website edits and order transitions')

    fields = {'Product.Name': 'Uploaded sample', 'Product.Price': '25', 'Product.Unit': 'pack',
              'Product.Stock': '10', 'Product.CategoryId': '1', 'Product.Description': 'Upload test',
              'Product.ShortDescription': 'Sample'}
    page = admin.post('/Product/Upsert', fields)
    assert 'Choose a product image.' in page
    token = Inputs(admin.request('/Product/Upsert')).fields['__RequestVerificationToken']
    fields['__RequestVerificationToken'] = token
    boundary = 'MudiSmokeBoundary'
    body = b''
    for key, value in fields.items():
        body += f'--{boundary}\r\nContent-Disposition: form-data; name="{key}"\r\n\r\n{value}\r\n'.encode()
    image = ROOT / 'Mudi/wwwroot/images/product/bb559035-fbbe-4ab9-8326-39be566d229d.png'
    body += f'--{boundary}\r\nContent-Disposition: form-data; name="files"; filename="sample.png"\r\nContent-Type: image/png\r\n\r\n'.encode()
    body += image.read_bytes() + f'\r\n--{boundary}--\r\n'.encode()
    admin.request('/Product/Upsert', body=body, content_type=f'multipart/form-data; boundary={boundary}')
    with sqlite3.connect(database) as db:
        product_id, filename = db.execute("SELECT Id, Image FROM Product WHERE Name='Uploaded sample'").fetchone()
    uploaded = ROOT / 'Mudi/wwwroot/images/product' / filename
    try:
        assert uploaded.is_file(), 'Image was not saved in the Linux web root'
        guest.request('/images/product/' + filename)
        admin.post(f'/Product/Upsert/{product_id}', dict(fields, **{'Product.Id': product_id, 'Product.Price': 30}))
        with sqlite3.connect(database) as db:
            assert db.execute('SELECT Price, Image FROM Product WHERE Id=?', (product_id,)).fetchone() == (30, filename)
        admin.post(f'/Product/Delete/{product_id}', {'Id': product_id})
        assert not uploaded.exists(), 'Deleting a product left its image behind'
    finally:
        uploaded.unlink(missing_ok=True)
    guest.post('/Home/ContactUs', {'ApplicationUser.FullName': 'Visitor',
        'ApplicationUser.Email': 'visitor@example.test', 'ApplicationUser.PhoneNumber': '01700000001',
        'Subject': 'Smoke contact', 'Message': 'Local contact test'})
    assert len(list(mail_dir.glob('*.html'))) == 3
    print('PASS missing-image validation, Linux upload/delete and contact email')


def main():
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    base = f'http://127.0.0.1:{port}'
    with tempfile.TemporaryDirectory(prefix='mudi-smoke-') as directory:
        directory = Path(directory)
        database = directory / 'test.db'
        mail_dir = directory / 'mail'
        env = dict(os.environ, ConnectionStrings__SqliteConnection=f'Data Source={database}',
                   Email__PickupDirectory=str(mail_dir))
        with (directory / 'server.log').open('w+') as log:
            server = subprocess.Popen([str(ROOT / 'scripts/run-local.sh'), '--no-build', '--no-restore',
                '--', '--urls', base], cwd=ROOT, env=env, stdout=log, stderr=log, start_new_session=True)
            try:
                for _ in range(100):
                    if server.poll() is not None:
                        log.seek(0)
                        raise RuntimeError(log.read())
                    try:
                        urllib.request.urlopen(base, timeout=1).close()
                        break
                    except (urllib.error.URLError, TimeoutError, socket.timeout):
                        time.sleep(0.2)
                else:
                    raise RuntimeError('Server did not start')
                run(base, database, mail_dir)
            finally:
                import signal
                if server.poll() is None:
                    os.killpg(server.pid, signal.SIGTERM)
                    server.wait(timeout=10)
                log.seek(0)
                contents = log.read()
                if 'fail:' in contents or 'crit:' in contents:
                    print(contents[-5000:])
    print('All local smoke checks passed. Disposable test data removed.')


if __name__ == '__main__':
    main()

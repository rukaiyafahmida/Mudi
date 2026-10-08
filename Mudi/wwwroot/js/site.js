(() => {
  'use strict';
  const themeButtons = document.querySelectorAll('[data-theme-toggle]');
  const updateTheme = () => {
    const light = document.documentElement.dataset.theme === 'light';
    themeButtons.forEach(button => {
      button.setAttribute('aria-label', `Switch to ${light ? 'dark' : 'light'} mode`);
      button.querySelector('[data-theme-label]')?.replaceChildren(`${light ? 'Dark' : 'Light'} mode`);
    });
    document.querySelector('meta[name="theme-color"]').content = light ? '#f6f4ef' : '#0b0e13';
  };
  updateTheme();
  themeButtons.forEach(button => button.addEventListener('click', () => {
    const next = document.documentElement.dataset.theme === 'light' ? 'dark' : 'light';
    document.documentElement.dataset.theme = next;
    try { localStorage.setItem('mudi-theme', next); } catch {}
    updateTheme();
  }));
  window.addEventListener('storage', e => { if (e.key === 'mudi-theme') { document.documentElement.dataset.theme = e.newValue === 'light' ? 'light' : 'dark'; updateTheme(); } });
  const toggle = document.querySelector('#menu-toggle');
  const rail = document.querySelector('#site-navigation');
  const backdrop = document.querySelector('.nav-backdrop');
  const mobile = window.matchMedia('(max-width: 900px)');
  function menu(open) {
    document.body.classList.toggle('nav-open', open);
    toggle?.setAttribute('aria-expanded', String(open));
    backdrop.hidden = !open;
    if (open) rail.querySelector('a, button')?.focus();
    else toggle?.focus();
  }
  toggle?.addEventListener('click', () => menu(!document.body.classList.contains('nav-open')));
  document.querySelector('.rail-close')?.addEventListener('click', () => menu(false));
  backdrop?.addEventListener('click', () => menu(false));
  document.addEventListener('keydown', e => {
    if (!document.body.classList.contains('nav-open')) return;
    if (e.key === 'Escape') menu(false);
    if (e.key === 'Tab') {
      const items = [...rail.querySelectorAll('a[href],button,input')].filter(el => !el.disabled && el.offsetParent !== null);
      if (e.shiftKey && document.activeElement === items[0]) { e.preventDefault(); items.at(-1).focus(); }
      else if (!e.shiftKey && document.activeElement === items.at(-1)) { e.preventDefault(); items[0].focus(); }
    }
  });
  mobile.addEventListener('change', () => {
    if (!mobile.matches) { document.body.classList.remove('nav-open'); backdrop.hidden = true; toggle?.setAttribute('aria-expanded','false'); }
  });
  const catalogue = document.querySelector('[data-catalogue]');
  if (catalogue) {
    const search = document.querySelector('#catalogue-search');
    const buttons = [...document.querySelectorAll('[data-category-filter]')];
    const feedback = document.querySelector('#search-feedback');
    const empty = document.querySelector('#catalogue-empty');
    let category = 'all';
    const filter = () => {
      const term = search.value.trim().toLocaleLowerCase();
      let count = 0;
      catalogue.querySelectorAll('.shelf').forEach(shelf => {
        let shown = 0;
        shelf.querySelectorAll('.product-tile').forEach(tile => {
          const visible = (category === 'all' || tile.dataset.category === category) && tile.dataset.search.toLocaleLowerCase().includes(term);
          tile.hidden = !visible;
          if (visible) { shown++; count++; }
        });
        shelf.hidden = shown === 0;
        shelf.querySelector('[data-shelf-count]').textContent = `${shown} ${shown === 1 ? 'product' : 'products'}`;
      });
      empty.hidden = count !== 0;
      feedback.hidden = !term && category === 'all';
      feedback.textContent = `${count} ${count === 1 ? 'product' : 'products'}${term ? ` matching “${search.value.trim()}”` : ''}`;
    };
    search.addEventListener('input', filter);
    buttons.forEach(button => button.addEventListener('click', () => {
      category = button.dataset.categoryFilter;
      buttons.forEach(b => b.setAttribute('aria-pressed', String(b === button)));
      filter();
    }));
    document.querySelector('#reset-catalogue')?.addEventListener('click', () => { search.value = ''; category = 'all'; buttons.forEach(b => b.setAttribute('aria-pressed', String(b.dataset.categoryFilter === 'all'))); filter(); search.focus(); });
    document.querySelectorAll('[data-shelf-move]').forEach(button => {
      const shelf = button.closest('.shelf').querySelector('.product-shelf');
      const updateScroll = () => {
        const atStart = shelf.scrollLeft <= 2;
        const atEnd = shelf.scrollLeft + shelf.clientWidth >= shelf.scrollWidth - 2;
        button.disabled = Number(button.dataset.shelfMove) < 0 ? atStart : atEnd;
      };
      shelf.addEventListener('scroll', updateScroll, {passive:true});
      new ResizeObserver(updateScroll).observe(shelf);
      button.addEventListener('click', () => shelf.scrollBy({left:Number(button.dataset.shelfMove) * shelf.clientWidth * .8,behavior:matchMedia('(prefers-reduced-motion: reduce)').matches?'instant':'smooth'}));
    });
    catalogue.querySelectorAll('.product-art').forEach(link => link.addEventListener('focus', () => link.closest('.product-tile').scrollIntoView({block:'nearest',inline:'nearest'})));
  }
  const upload = document.querySelector('#uploadBox');
  const preview = document.querySelector('#product-preview');
  let imageUrl;
  upload?.addEventListener('change', () => {
    if (!upload.files[0] || !upload.files[0].type.startsWith('image/')) return;
    if (imageUrl) URL.revokeObjectURL(imageUrl);
    imageUrl = URL.createObjectURL(upload.files[0]);
    preview.src = imageUrl; preview.hidden = false;
    document.querySelector('#image-placeholder')?.setAttribute('hidden', '');
  });
})();

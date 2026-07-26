export function applyTheme(theme) {
	document.documentElement.setAttribute('data-theme', theme);
	localStorage.setItem('theme', theme);
}

export function loadTheme() {
	const savedTheme = localStorage.getItem('theme');
	const defaultTheme = window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
	
	return savedTheme || defaultTheme;
}

export function setTheme(theme) {
	localStorage.setItem('theme', theme);
}
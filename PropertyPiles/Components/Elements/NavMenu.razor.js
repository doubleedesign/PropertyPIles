export class NavMenu {
	linkToSectionMap;
	sections;
	
	constructor() {
		this.init();
	}
	
	init() {
		this.sections = document.querySelectorAll("section");
		const sectionIds = Array.from(this.sections).map((el) => el.getAttribute("id"));
		const menuLinks = Array.from(document.querySelectorAll("nav a")).map(item => item.getAttribute('href').replace("#", ""));

		// By design, not all sections have a matching menu link.
		// In that case, it should use the previous one (e.g., "Maybe" goes to #shortlist, the same as "Priority")
		this.linkToSectionMap = Array.from(this.sections).reduce((acc, section) => {
			const sectionId = section.id;

			if(!menuLinks.includes(sectionId)) {
				const sectionIndex = sectionIds.findIndex(section => section === sectionId);
				const previousOrFirst = sectionIndex > 0 ? sectionIds[sectionIndex - 1] : sectionIds[0];
				acc[sectionId] = previousOrFirst;

				return acc;
			}

			acc[sectionId] = sectionId;
			return acc;
		}, {});
	}
	
	getFirstSection() {
		return Object.keys(this.linkToSectionMap)[0];
	}
	
	getActiveSectionOnPageLoad() {
		const hash = window.location.hash;
		if(!hash) return this.getFirstSection();

		const anchor = hash.replace("#", "");
		return this.linkToSectionMap[anchor] || this.getFirstSection();
	}
	
	getActiveSection() {
		let closestSectionId = "";
		let minDistanceToTop = window.innerHeight;
		this.sections.forEach((section) => {
			const sectionTop = section.getBoundingClientRect().top;
			if (sectionTop >= 0 && sectionTop < minDistanceToTop) {
				minDistanceToTop = sectionTop;
				closestSectionId = section.getAttribute('id');
			}
		});

		return this.linkToSectionMap[closestSectionId] || this.getFirstSection();
	}
}

export function registerEventListeners(dotNetRef) {
	const instance = new NavMenu(dotNetRef);
	dotNetRef.invokeMethodAsync('SetActiveAnchor', instance.getActiveSectionOnPageLoad());

	const handler = (event) => {
		if(instance.getActiveSection() !== "") {
			dotNetRef.invokeMethodAsync('OnScroll', instance.getActiveSection());
			setTimeout(() => {
				history.pushState(null, '', `#${instance.getActiveSection()}`);
			}, 300);
		}
	} 
	
	window.addEventListener('scrollend', handler);
	
	// Return a cleanup function reference
	return handler;
}

export function unregisterEventListeners(handler) {
	window.removeEventListener('scrollend', handler);
}
export function registerEventListeners(dotNetRef) {
	const handler = (event) => {
		console.log('scroll event');
		dotNetRef.invokeMethodAsync('OnScroll', getActiveSection());
	} 
	
	window.addEventListener('scrollend', handler);
	
	// Return a cleanup function reference
	return handler;
}

function getActiveSection() {
	const sections = document.querySelectorAll("section");
	const sectionIds = Array.from(sections).map((el) => el.getAttribute("id"));
	const menuLinks = Array.from(document.querySelectorAll("nav a")).map(item => item.getAttribute('href').replace("#", ""));

	// By design, not all sections have a matching menu link.
	// In that case, it should use the previous one (e.g., "Maybe" goes to #shortlist, the same as "Priority")
	// Also, the first one should go to #top to account for the sticky header
	const linkToSectionPairs = Array.from(sections).reduce((acc, section) => {
		const sectionId = section.id;
		const sectionIndex = sectionIds.findIndex(section => section === sectionId);
		
		if(sectionIndex === 0) {
			acc[sectionId] = "top";
			return acc;
		}
		
		if(!menuLinks.includes(sectionId)) {
			const sectionIndex = sectionIds.findIndex(section => section === sectionId);
			const previousOrFirst = sectionIndex > 0 ? sectionIds[sectionIndex - 1] : 0;
			acc[sectionId] = previousOrFirst === 0 ? "top" : sectionIds[previousOrFirst];
			
			return acc;
		}
		
		acc[sectionId] = sectionId;
		return acc;
	}, {});
	
	let closestSectionId = "";
	let minDistanceToTop = window.innerHeight;
	sections.forEach((section) => {
		const sectionTop = section.getBoundingClientRect().top;
		if (sectionTop >= 0 && sectionTop < minDistanceToTop) {
			minDistanceToTop = sectionTop;
			closestSectionId = section.getAttribute('id');
		}
	});
	
	return linkToSectionPairs[closestSectionId] || closestSectionId;
}


export function unregisterScrollListener(handler) {
	window.removeEventListener('scrollend', handler);
}

export function activeMenuItemClass() {
	
	const sections = document.querySelectorAll("section");
	const menuLinks = document.querySelectorAll(".site-header a");
	let closestSectionId = "";
	let minDistanceToTop = window.innerHeight;

	sections.forEach((section) => {
		const sectionTop = section.getBoundingClientRect().top;
		if (sectionTop >= 0 && sectionTop < minDistanceToTop) {
			minDistanceToTop = sectionTop;
			closestSectionId = section.getAttribute('id');
		}
	});

	menuLinks.forEach((link) => {
		link.classList.remove('active');
		if (link.getAttribute('href') === `#${closestSectionId}`) {
			link.classList.add('active');
		}
	});
}
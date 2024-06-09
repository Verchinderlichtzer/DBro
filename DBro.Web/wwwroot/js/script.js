window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.updateUrl = () => {
    let sections = document.querySelectorAll('section');
    let navLinks = document.querySelectorAll('.mud-list-item');

    window.onscroll = () => {
        sections.forEach(sec => {
            let top = window.scrollY;
            let offset = sec.offsetTop - 150;
            let height = sec.offsetHeight;
            let id = sec.getAttribute('id');

            if (top >= offset && top < offset + height) {
                navLinks.forEach(link => {
                    link.classList.remove('mud-selected-item', 'mud-primary-text', 'mud-primary-hover');
                    document.querySelector(`#nav-${id}`).classList.add('mud-selected-item', 'mud-primary-text', 'mud-primary-hover');
                })
            }
        })
    }
}

function scrollToSection(id) {
    var element = document.getElementById(id);
    var headerOffset = 45;
    var elementPosition = element.getBoundingClientRect().top;
    var offsetPosition = elementPosition + window.scrollY - headerOffset;

    window.scrollTo({
        top: offsetPosition,
        behavior: "smooth"
    });
}
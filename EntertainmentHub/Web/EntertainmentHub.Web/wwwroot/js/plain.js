function ReadMore() {
    const allowedLength = 700;
    const strongEl = '<strong class="str-col">Biography: </strong>'

    let paragraph = document.querySelector('.readMore');
    let text = paragraph.textContent;
    paragraph.innerHTML = `${strongEl} ${text.substring(0, allowedLength)}`;

    if (text.length > allowedLength) {
        paragraph.innerHTML += '<a style="cursor:pointer">... Read More</a>';
        paragraph.querySelector('a').addEventListener('click', clickLink);
    }

    function clickLink(ev) {
        if (ev.target.textContent == '... Read More') {
            paragraph.innerHTML = `${strongEl} ${text} <a style="cursor:pointer"> Read Less</a>`;
        }
        else {
            paragraph.innerHTML = `${strongEl} ${text.substring(0, allowedLength)} <a style="cursor:pointer">... Read More</a>`;
            window.scrollTo(290, 290);
        }

        paragraph.querySelector('a').addEventListener('click', clickLink);
    }
}

ReadMore();
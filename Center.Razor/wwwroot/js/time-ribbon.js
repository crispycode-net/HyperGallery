export class timeRibbon {
    hasFocus = false;
    yearContainer = null;
    yearItems = [];
    animationsPlaying = 0;
    focusedYearItemIndex = null;
    yearSelectedCallback = null;

    constructor(yearContainerId, yearSelectedCallback) {
        this.yearContainer = document.getElementById(yearContainerId);
        this.yearSelectedCallback = yearSelectedCallback;
    }

    async initAsync() {
        let yearRequest = await fetch("/api/MediaItem/Years");
        let years = await yearRequest.json();

        this.hasFocus = true;

        this.yearItems = [];
        years.forEach(y => {
            const yi = this.createYearItem(y);
            yearContainer.appendChild(yi.element);
            this.yearItems.push(yi);
        });

        this.focusedYearItemIndex = 0;
        this.focusYearItem(this.focusedYearItemIndex);        

        let yearRibbon = this;
        document.addEventListener('keydown', function (e) {
            if (e.key === "ArrowRight") {
                yearRibbon.moveCursorHorizontally(e, true);
            } else if (e.key === "ArrowLeft") {
                yearRibbon.moveCursorHorizontally(e, false);
            } else if (e.key === "Enter") {
                yearRibbon.yearSelectedCallback(yearRibbon.yearItems[yearRibbon.focusedYearItemIndex].year);
                yearRibbon.unfocusRibbon();
            }
        });

        return this.yearItems[0].year;
    }

    unfocusRibbon() {
        this.hasFocus = false;
        this.yearItems.forEach(yi => {
            if (yi != this.yearItems[this.focusedYearItemIndex]) {
                yi.element.classList.add("hidden");
            }
        });
    }

    createYearItem(year) {
        let yearItem = {
            year: year,
            element: document.createElement('h1')
        }
        yearItem.element.innerText = "" + year;

        return yearItem;
    }

    focusYearItem(yearItemIndex) {
        if (this.focusedYearItemIndex != null)
            this.yearItems[this.focusedYearItemIndex].element.classList.remove("focused");

        this.focusedYearItemIndex = yearItemIndex;
        this.yearItems[this.focusedYearItemIndex].element.classList.add("focused");        
    }

    moveCursorHorizontally(e, moveRight) {
        if (!this.hasFocus)
            return;

        if (this.animationsPlaying > 0)
            return;

        if (this.focusedYearItemIndex === null)
            return;

        if (moveRight && this.focusedYearItemIndex >= this.yearItems.length - 1)
            return;   

        if (!moveRight && this.focusedYearItemIndex <= 0)
            return;        

        this.focusYearItem(moveRight ? this.focusedYearItemIndex + 1 : this.focusedYearItemIndex - 1);
    }
}
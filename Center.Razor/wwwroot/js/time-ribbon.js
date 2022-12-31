export class timeRibbon {
    yearContainer = null;
    yearItems = [];
    focusedYearItemIndex = null;
    initialFocusedYearItemIndex = null;

    constructor(yearContainer) {
        this.yearContainer = yearContainer;
    }

    async initAsync() {
        let yearRequest = await fetch("/api/MediaItem/Years");
        let years = await yearRequest.json();

        this.yearItems = [];
        years.forEach(y => {
            const yi = this.createYearItem(y);
            yearContainer.appendChild(yi.element);
            this.yearItems.push(yi);
        });

        this.focusedYearItemIndex = 0;
        this.focusYearItem(this.focusedYearItemIndex);

        return this.yearItems[0].year;
    }

    createYearItem(year) {
        let yearItem = {
            year: year,
            element: document.createElement('h1')
        }
        yearItem.element.innerText = "" + year;

        return yearItem;
    }

    activateView() {
        this.initialFocusedYearItemIndex = this.focusedYearItemIndex;
        this.yearItems.forEach(yi => {
            if (yi != this.yearItems[this.focusedYearItemIndex]) {
                yi.element.classList.remove("hidden");
            }
        });
    } 

    deactivateView() {
        this.yearItems.forEach(yi => {
            if (yi != this.yearItems[this.focusedYearItemIndex]) {
                yi.element.classList.add("hidden");
            }
        });
    }

    focusYearItem(yearItemIndex) {
        if (this.focusedYearItemIndex != null)
            this.yearItems[this.focusedYearItemIndex].element.classList.remove("focused");

        this.focusedYearItemIndex = yearItemIndex;

        const element = this.yearItems[this.focusedYearItemIndex].element;
        element.classList.add("focused");    
        element.scrollIntoView();    
    }

    moveCursorHorizontally(moveRight) {
        if (this.focusedYearItemIndex === null)
            return;

        if (moveRight && this.focusedYearItemIndex >= this.yearItems.length - 1)
            return;   

        if (!moveRight && this.focusedYearItemIndex <= 0)
            return;        

        this.focusYearItem(moveRight ? this.focusedYearItemIndex + 1 : this.focusedYearItemIndex - 1);
    }

    getFocusedYear() {
        return this.yearItems[this.focusedYearItemIndex].year;
    }
}
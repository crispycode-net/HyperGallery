import { imageWall } from './image-wall.js';
import { timeRibbon } from './time-ribbon.js';

export class mediaSchnaff {

    yearContainer = null;
    displayElement = null;
    imageWall = null;
    timeRibbon = null;
    activeComponent = null;

    constructor(yearContainerId, displayElementId) {
        this.displayElement = document.getElementById(displayElementId);
        this.yearContainer = document.getElementById(yearContainerId);

        this.imageWall = new imageWall(this.displayElement);
        this.timeRibbon = new timeRibbon(this.yearContainer);
        this.activeComponent = this.timeRibbon;

        let ms = this;
        document.addEventListener('keydown', function (e) {
            if (e.key === "ArrowRight") {
                ms.handleArrowRight(e);
            } else if (e.key === "ArrowLeft") {
                ms.handleArrowLeft(e);
            } else if (e.key === "ArrowUp") {
                ms.handleArrowUp(e);
            } else if (e.key === "ArrowDown") {
                ms.handleArrowDown(e);
            } else if (e.key === "Enter") {
                ms.handleEnter(e);
            } else if (e.key === "Escape") {
                ms.handleEscape(e);
            }
        });
    }

    async initAsync() {
        await this.timeRibbon.initAsync();
    }

    handleEnter(e) {
        if (this.activeComponent == this.timeRibbon) {
            this.timeRibbon.deactivateView();
            this.imageWall.init(this.timeRibbon.getFocusedYear());
            this.activeComponent = this.imageWall;
        }
        else if (this.activeComponent == this.imageWall) {
            this.imageWall.handleEnter();
        }        
    }

    handleEscape(e) {
        if (this.activeComponent == this.timeRibbon) {
            this.cancelTimeRibbonNavigation();
        }
        else if (this.activeComponent == this.imageWall) {
            this.imageWall.handleEscape();
        }         
    }

    cancelTimeRibbonNavigation() {
        if (this.timeRibbon.initialFocusedYearItemIndex == null)
            return;
            
        this.timeRibbon.focusYearItem(this.timeRibbon.initialFocusedYearItemIndex);
        this.timeRibbon.deactivateView();
        this.activeComponent = this.imageWall;
    }

    handleArrowRight(e) {
        if (this.activeComponent == this.timeRibbon) {            
            this.timeRibbon.moveCursorHorizontally(true);
        }
        else if (this.activeComponent == this.imageWall) {
            this.imageWall.moveCursorHorizontally(e, true);
        }
    }

    handleArrowLeft(e) {
        if (this.activeComponent == this.timeRibbon) {            
            this.timeRibbon.moveCursorHorizontally(false);
        }
        else if (this.activeComponent == this.imageWall) {
            this.imageWall.moveCursorHorizontally(e, false);            
        }
    }

    handleArrowUp(e) {
        if (this.activeComponent == this.timeRibbon) {
        }
        else if (this.activeComponent == this.imageWall) {
            let movedOutsideofImageWall = this.imageWall.moveCursorVertically(e, false);
            if (movedOutsideofImageWall === true) {
                this.timeRibbon.activateView();
                this.activeComponent = this.timeRibbon;
            }
        }
    }

    handleArrowDown(e) {
        if (this.activeComponent == this.timeRibbon) {
            this.cancelTimeRibbonNavigation();
        }
        else if (this.activeComponent == this.imageWall) {
            this.imageWall.moveCursorVertically(e, true);
        }
    }
}
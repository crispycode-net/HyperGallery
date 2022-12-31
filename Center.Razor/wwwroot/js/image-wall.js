export class imageWall {
    displayElement = null;
    firstColumnOffset = 0;
    columnWidth = 400;
    rowHeight = 200;
    animationsPlaying = 0;
    focusedMediaItem = null;
    visibleColumns = 4;
    visibleRows = 3;
    currentYearItems = [];
    loadedMediaItems = [];
    initialInitDone = false;
    fullScreenView = null;    

    constructor(displayElement) {
        this.displayElement = displayElement;
    }

    async init(currentYear) {        

        this.loadedMediaItems = [];
        this.firstColumnOffset = 0;
        this.animationsPlaying = 0;
        while (this.displayElement.firstChild) {
            this.displayElement.removeChild(this.displayElement.lastChild);
        }

        let yearRequest = await fetch("/api/Years/" + currentYear);
        this.currentYearItems = await yearRequest.json();

        const domRect = this.displayElement.getBoundingClientRect();
        this.columnWidth = parseInt(domRect.width / this.visibleColumns);
        this.rowHeight = parseInt(domRect.height / this.visibleRows);

        for (let c = 0; c < this.visibleColumns + 1; c++) {
            for (let r = 0; r < this.visibleRows; r++) {
                let dataIndex = c * this.visibleRows + r;
                let rect = { x: c * this.columnWidth, y: r * this.rowHeight, w: this.columnWidth, h: this.rowHeight };
                let mediaItem = this.createMediaItem(dataIndex, rect, c, r);
                this.loadedMediaItems.push(mediaItem);
                this.displayElement.append(mediaItem.img);
            }
        }

        this.focusMediaItem(this.loadedMediaItems[0]);        

        this.initCSS(this.columnWidth);

        if (this.initialInitDone === false) {
            this.displayElement.style.overflow = "hidden";
        }

        this.initialInitDone = true;
    }

    initCSS(columnWidthInPx) {
        var style = document.getElementById("keyframe-styles");
        if (style) 
            style.remove();

        var style = document.createElement('style');
        var keyFrames = '\
    @keyframes shiftLeftDyn {\
        from {\
            margin-left: 0px;\
        }\
        to {\
            margin-left: -A_DYNAMIC_VALUEpx;\
        }\
    }\
    \
    @keyframes shiftRightDyn {\
        from {\
            margin-left: 0px;\
        }\
        to {\
            margin-left: A_DYNAMIC_VALUEpx;\
        }\
    }\ ';
        style.id = "keyframe-styles";
        style.innerHTML = keyFrames.replace(/A_DYNAMIC_VALUE/g, "" + columnWidthInPx);
        document.getElementsByTagName('head')[0].appendChild(style);
    }

    createMediaItem(dataIndex, rect, gridColumnIndex, gridRowIndex) {
        let mediaItem = {
            dataIndex: dataIndex,
            rect: rect,
            img: document.createElement('img'),
            gridColumnIndex: gridColumnIndex,
            gridRowIndex: gridRowIndex,
            kind: this.currentYearItems[dataIndex].kind,
            id: this.currentYearItems[dataIndex].id
        };

        mediaItem.img.id = "mi_" + dataIndex;
        mediaItem.img.src = this.currentYearItems[dataIndex].path;
        mediaItem.img.style.position = "absolute";
        mediaItem.img.style.left = rect.x + "px";
        mediaItem.img.style.top = rect.y + "px";
        mediaItem.img.style.width = rect.w + "px";
        mediaItem.img.style.height = rect.h + "px";
        mediaItem.img.style.objectFit = "cover";
        mediaItem.img.mediaItem = mediaItem;

        if (mediaItem.kind === "video") {
            mediaItem.img.style.border = "1px solid white";
        }

        return mediaItem;
    }

    focusMediaItem(mediaItem) {
        if (this.focusedMediaItem != null)
            this.focusedMediaItem.img.classList.remove("focused-media-item");

        mediaItem.img.classList.add("focused-media-item");
        this.focusedMediaItem = mediaItem;
    }

    moveCursorHorizontally(e, moveRight) {
        if (this.fullScreenView != null)
            return;

        if (this.animationsPlaying > 0)
            return;

        if (moveRight) {
            if (this.focusedMediaItem.gridColumnIndex < this.visibleColumns - 1) {
                this.focusMediaItemByOffset(this.visibleRows);
                return;
            }
        }
        else {
            if (this.focusedMediaItem.gridColumnIndex > 0) {
                this.focusMediaItemByOffset(-this.visibleRows);
                return;
            }
        }

        this.scroll(e.repeat === false, moveRight);

        this.focusMediaItemByOffset(moveRight ? this.visibleRows : -this.visibleRows);
    }

    /** Returns true if we're leaving the navigation area of the image wall */
    moveCursorVertically(e, moveDown) {
        if (this.fullScreenView != null)
            return false;

        if (!moveDown && this.focusedMediaItem.gridRowIndex == 0)         
            return true;

        this.focusMediaItemByOffset(moveDown ? 1 : -1);
        return false;
    }

    focusMediaItemByOffset(offset) {
        let rightNeighbor = this.loadedMediaItems.find(mi => mi.dataIndex == this.focusedMediaItem.dataIndex + offset);
        if (rightNeighbor)
            this.focusMediaItem(rightNeighbor);
    }

    scroll(withAnimation, moveRight) {

        if (moveRight && (this.firstColumnOffset + this.visibleColumns) * 2 >= this.currentYearItems.length)
            return;

        if (!moveRight && this.firstColumnOffset <= 0)
            return;

        this.shiftLoadedMediaItemsByOneGridColumn(withAnimation, moveRight);

        this.loadedMediaItems = this.loadedMediaItems.filter(mediaItem => mediaItem.gridColumnIndex >= -1 && mediaItem.gridColumnIndex <= this.visibleColumns);

        if (moveRight) {
            this.firstColumnOffset++;
            this.appendNewItems(this.firstColumnOffset + this.visibleColumns, true);
        }
        else {
            this.firstColumnOffset--;
            this.appendNewItems(this.firstColumnOffset - 1, false);
        }
    }

    shiftLoadedMediaItemsByOneGridColumn(withAnimation, moveRight) {
        this.loadedMediaItems.forEach(mediaItem => {

            if (moveRight) {
                mediaItem.rect.x -= this.columnWidth;
                mediaItem.gridColumnIndex -= 1;
            }
            else {
                mediaItem.rect.x += this.columnWidth;
                mediaItem.gridColumnIndex += 1;
            }

            if (mediaItem.gridColumnIndex < -1 || mediaItem.gridColumnIndex > this.visibleColumns) {
                mediaItem.img.remove();
            }
            else {
                if (withAnimation) {
                    mediaItem.img.addEventListener('animationend', this.moveAnimationCompleted.bind(this));
                    if (moveRight)
                        mediaItem.img.style.animation = "shiftLeftDyn 0.15s 1";
                    else
                        mediaItem.img.style.animation = "shiftRightDyn 0.15s 1";
                    this.animationsPlaying++;
                }
                else {
                    mediaItem.img.style.left = mediaItem.rect.x + "px";
                }
            }
        });
    }

    appendNewItems(newLastVisibleColumn, addRight) {
        for (let r = 0; r < this.visibleRows; r++) {
            let dataIndex = newLastVisibleColumn * this.visibleRows + r;

            if (dataIndex < 0 || dataIndex >= this.currentYearItems.length)
                return;

            let loadingIsRequired = true;
            this.loadedMediaItems.forEach(mi => {
                if (mi.dataIndex == dataIndex) {
                    loadingIsRequired = false;
                    return;
                }
            });

            if (!loadingIsRequired)
                return;

            let addToGridColumn = -1;
            if (addRight)
                addToGridColumn = this.visibleColumns;

            let rect = { x: addToGridColumn * this.columnWidth, y: r * this.rowHeight, w: this.columnWidth, h: this.rowHeight };
            let mediaItem = this.createMediaItem(dataIndex, rect, addToGridColumn, r);
            this.loadedMediaItems.push(mediaItem);
            this.displayElement.append(mediaItem.img);
        }
    }

    moveAnimationCompleted(e) {
        let mediaItem = e.srcElement.mediaItem;
        mediaItem.img.style.left = mediaItem.rect.x + "px";
        e.srcElement.style.animation = null;

        this.animationsPlaying--;
    }

    createFullScreenView(focusedMediaItem) {

        let fullScreenView = document.createElement('div');
        fullScreenView.style.position = "absolute";
        fullScreenView.style.left = "0px";
        fullScreenView.style.top = "0px";
        fullScreenView.style.width = "calc(100vW - 0px)";
        fullScreenView.style.height = "calc(100vH - 0px)";
        fullScreenView.style.backgroundColor = "black";
        fullScreenView.style.overflow = "hidden";

        if (focusedMediaItem.kind === "video") {
            let video = document.createElement("video");
            video.style.width = "100%";
            video.style.height = "100%";
            video.src = "/api/Video/" + focusedMediaItem.id;
            video.autoplay = true;
            fullScreenView.appendChild(video);
        } else {
            let img = document.createElement("img");
            img.style.width = "100%";
            img.style.height = "100%";
            img.style.objectFit = "contain";
            img.src = "/api/Photo/" + focusedMediaItem.id;
            fullScreenView.appendChild(img);
        }

        return fullScreenView;
    }

    handleEnter() {
        if (this.fullScreenView != null)
            return;

        this.fullScreenView = this.createFullScreenView(this.focusedMediaItem);
        document.body.appendChild(this.fullScreenView);
    }

    handleEscape() {
        if (this.fullScreenView == null)
            return;
        
        this.fullScreenView.remove();
        this.fullScreenView = null;
    }
}
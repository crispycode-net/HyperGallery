let firstColumnOffset = 0;
let columnWidth = 400;
let rowHeight = 200;
let animationsPlaying = 0;
let focusedMediaItem = null;

const visibleColumns = 4;
const visibleRows = 3;

let currentYearItems = [
    { path: "Thumbnails/2017/2017-12-05_3cfe6c2d-0a26-4fbe-baa4-8c953a969d4e.jpg"},
    { path: "Thumbnails/2017/2017-12-06_76047bdc-9c7e-41d2-8de5-6bcf4e3c5034.jpg"},
    { path: "Thumbnails/2017/2017-12-07_7cc63ac9-e77e-4114-b370-f307eb883e02.jpg"},
    { path: "Thumbnails/2017/2017-12-08_bbc83edc-1c2b-414c-bd59-824946bc9348.jpg"},
    { path: "Thumbnails/2017/2017-12-08_c1fbd61b-e5da-4f90-acf7-a1bfc4fed017.jpg"},
    { path: "Thumbnails/2017/2017-12-10_c1f3ae62-ed82-4325-a927-5ee34697ef37.jpg"},
    { path: "Thumbnails/2017/2017-12-11_f6ec466f-2912-4287-8d43-fc8e74d046f2.jpg"},
    { path: "Thumbnails/2017/2017-12-15_30567106-9381-478c-8e2e-51a6c116f2d8.jpg"},
    { path: "Thumbnails/2017/2017-12-17_62334eb4-f897-49da-8843-54bd356d2797.jpg"},
    { path: "Thumbnails/2017/2017-12-20_8b258fa5-95c1-40b7-b557-541ee28cfd83.jpg"},
    { path: "Thumbnails/2017/2017-12-20_bbf7b6e9-0518-40b2-862c-2714ee8b7dca.jpg"},
    { path: "Thumbnails/2017/2017-12-22_a250ba6b-f907-4498-afeb-268671d0882e.jpg"},
    { path: "Thumbnails/2017/2017-12-22_e9b69272-c81b-4fae-9d29-cc321853efbc.jpg"},
    { path: "Thumbnails/2017/2017-12-23_08a6b467-fdec-4c09-ac78-83325c4ea796.jpg"},
    { path: "Thumbnails/2017/2017-12-24_26b3e152-aabb-4310-b993-41308474014e.jpg"},
    { path: "Thumbnails/2017/2017-12-24_c526cec9-db43-40b4-8f61-2148360daee3.jpg"},
    { path: "Thumbnails/2017/2017-12-24_d985b8d2-9b05-4641-bc16-7defda133d0c.jpg"},
    { path: "Thumbnails/2017/2017-12-25_f3ff1519-d30d-4ec8-bf40-fdc108fc7b23.jpg"},
    { path: "Thumbnails/2017/2017-12-28_57189e50-599d-44c5-adc1-4ac55a9fadbc.jpg"},
    { path: "Thumbnails/2017/2017-12-30_5bc44065-92f7-44ca-ab80-3a152470b5d7.jpg"},
    { path: "Thumbnails/2017/2017-12-30_59493e88-9ed2-43c6-91a6-9445bf530bea.jpg"},
    { path: "Thumbnails/2017/2017-02-26_450d62ed-e297-436b-842c-6165524abe04.jpg"},
    { path: "Thumbnails/2017/2017-03-01_568686f3-da8b-439c-bc2b-74b32255661c.jpg"},
    { path: "Thumbnails/2017/2017-03-11_220963a0-78f0-4a2d-a7ff-17a160d4d2e3.jpg"},
    { path: "Thumbnails/2017/2017-03-12_680250e9-3d70-4c5d-a9e2-a245199f03d7.jpg"},
    { path: "Thumbnails/2017/2017-03-17_1d683f5a-1719-4282-97ae-e219f7453cc6.jpg"},
    { path: "Thumbnails/2017/2017-03-17_3e6532c7-78fc-4424-80ca-e1072e7ec35a.jpg"},
    { path: "Thumbnails/2017/2017-03-17_42cefc73-efb4-4acf-9057-aeaf9c4c17fb.jpg"},
    { path: "Thumbnails/2017/2017-03-17_75d20eba-e63f-440e-8863-15ed87c44f85.jpg"},
    { path: "Thumbnails/2017/2017-03-17_e2ddd4eb-7a81-44c0-b865-589f92ae5f66.jpg"},
    { path: "Thumbnails/2017/2017-03-18_fc07095a-6731-4b31-82b6-0dfc5bb10336.jpg"},
    { path: "Thumbnails/2017/2017-03-20_3efc8e80-1d33-4495-b3d3-0d5b790ce0fb.jpg"},
    { path: "Thumbnails/2017/2017-03-30_3c99bc4d-eeb4-4c0d-a18b-1c1c9abbb054.jpg"},
    { path: "Thumbnails/2017/2017-04-04_f5a47b7e-6b0b-4059-99d2-a299dc8547de.jpg"},
    { path: "Thumbnails/2017/2017-04-13_86cc3b40-f669-41f3-b3ef-2aec45e88224.jpg"},
    { path: "Thumbnails/2017/2017-04-13_4895a024-5e05-4514-b192-45fa56ea148a.jpg"},
    { path: "Thumbnails/2017/2017-04-13_f72d1a7d-83c0-437c-82d3-44d17fcc9f1a.jpg"},
    { path: "Thumbnails/2017/2017-04-14_366ff817-89ef-45ba-be93-653a06870d3e.jpg"},
    { path: "Thumbnails/2017/2017-04-15_0ccc68ec-b6dc-447f-8e84-4556f0079df6.jpg"},
    { path: "Thumbnails/2017/2017-04-15_01b7cb22-c066-40f7-aa58-31e0a3d1f1fd.jpg"},
    { path: "Thumbnails/2017/2017-04-15_67dbe7c4-65d4-4325-9f1d-2f676e898a9b.jpg"},
    { path: "Thumbnails/2017/2017-04-15_73a7832f-3c4f-4ece-a35f-1d4333b1cf53.jpg"},
    { path: "Thumbnails/2017/2017-04-15_c22caf9b-7e77-45f4-980b-79c0d0710156.jpg"},
    { path: "Thumbnails/2017/2017-04-15_c627d6fe-734c-415a-91cd-5cb8cc4fc4c6.jpg"},
    { path: "Thumbnails/2017/2017-04-30_5c9f9446-5d98-4c01-abb7-d488cf2c05c3.jpg"},
    { path: "Thumbnails/2017/2017-04-30_46bb36bf-6001-48f8-a284-eea7f75cca89.jpg"},
    { path: "Thumbnails/2017/2017-04-30_b5d8249b-59bf-46fa-8110-1ff2c770ca90.jpg"},
    { path: "Thumbnails/2017/2017-04-30_e2bb5ea2-35cc-4af4-ab1f-7aec52162d95.jpg"},
    { path: "Thumbnails/2017/2017-05-01_6c734281-577c-4c76-a115-3b06b96a1f40.jpg"},
    { path: "Thumbnails/2017/2017-05-01_7ffc8216-6c21-4c28-be3e-ddc72000aba9.jpg"},
    { path: "Thumbnails/2017/2017-05-19_26bd0812-fffd-4f23-a962-f3d653a0ce01.jpg"},
    { path: "Thumbnails/2017/2017-05-19_b48ace30-4955-4762-9b6d-b8962906fdfb.jpg"},
    { path: "Thumbnails/2017/2017-06-02_9c8a92da-ba96-45fb-a041-6ec4c81e885d.jpg"},
    { path: "Thumbnails/2017/2017-06-03_85f50988-ad13-4074-8e92-66a6d48f25a2.jpg"},
    { path: "Thumbnails/2017/2017-06-03_b9141ab8-ff98-426d-8b15-11c9a33ef545.jpg"},
    { path: "Thumbnails/2017/2017-06-03_e6f11b0f-a970-49f5-b37d-cc6ab5b15401.jpg"},
    { path: "Thumbnails/2017/2017-06-08_0f12155e-d17d-441e-97d8-5e8d7647ea94.jpg"},
    { path: "Thumbnails/2017/2017-06-08_d14543bc-0168-4e3a-bf1b-3c8d831b730f.jpg"},
    { path: "Thumbnails/2017/2017-06-09_4152c196-5c56-4646-a9d6-981f472ddffb.jpg"},
    { path: "Thumbnails/2017/2017-06-10_0d5e7925-3096-47fc-a8ae-810cb6f3a6c5.jpg"},
    { path: "Thumbnails/2017/2017-06-10_a8b4dcec-5cef-4629-b0d7-924fab02ae15.jpg"},
    { path: "Thumbnails/2017/2017-06-18_6e0bb21c-0108-4d68-a990-b3d22b65751d.jpg"},
    { path: "Thumbnails/2017/2017-06-18_37fc3a6e-bd5d-419f-97f3-a0aed9703118.jpg"},
    { path: "Thumbnails/2017/2017-06-18_a7d05064-9157-49b4-8aa3-803c2977ddd3.jpg"},
    { path: "Thumbnails/2017/2017-06-18_eadf1087-756c-46ab-8164-b13d071da17e.jpg"},
    { path: "Thumbnails/2017/2017-06-18_f774995a-f759-481c-b604-08b2b03f5fe1.jpg"},
    { path: "Thumbnails/2017/2017-06-18_fdd57dd3-77d7-43a6-bd9c-213d6b0062c3.jpg"},
    { path: "Thumbnails/2017/2017-06-19_39a02171-f166-4025-966b-d04d69a345fd.jpg"},
    { path: "Thumbnails/2017/2017-06-19_979600a9-99e8-4485-9c0f-0e5f4260d239.jpg"},
    { path: "Thumbnails/2017/2017-06-19_e1a46dc1-05eb-4860-8d01-b09246fe7287.jpg"},
    { path: "Thumbnails/2017/2017-06-21_02f393f6-bcc2-41be-8612-0162bcef3c0e.jpg"},
    { path: "Thumbnails/2017/2017-06-21_d9d6fbbf-f233-4f2b-bd3e-013d083187e3.jpg"},
    { path: "Thumbnails/2017/2017-06-22_3f3b7c57-ecb1-497a-b5a1-978b6a3eea4f.jpg"},
    { path: "Thumbnails/2017/2017-06-22_6d258801-6aff-4146-a917-bd87764267a9.jpg"},
    { path: "Thumbnails/2017/2017-06-22_12e135eb-28d0-47f4-80f2-514988ce090a.jpg"},
    { path: "Thumbnails/2017/2017-06-22_7625a5d1-0576-4336-a646-d78f1eee610b.jpg"},
    { path: "Thumbnails/2017/2017-06-23_4d7726ee-37e5-4831-af75-7ae218d9ab59.jpg"},
    { path: "Thumbnails/2017/2017-06-23_96fd4715-a9e5-4709-bf2a-d114b618ce85.jpg"},
    { path: "Thumbnails/2017/2017-06-23_a3f7696a-359b-49cc-aa44-dfb6ccd80028.jpg"},
    { path: "Thumbnails/2017/2017-06-23_f2b5d9c5-3b65-4f52-b305-8568b6a064e6.jpg"},
    { path: "Thumbnails/2017/2017-06-30_8c1c547f-e633-47d7-afee-b526f67bcbb5.jpg"},
    { path: "Thumbnails/2017/2017-07-04_331808f8-29b2-4da7-93fe-1fa459d019fc.jpg"},
    { path: "Thumbnails/2017/2017-07-10_df581145-4477-4410-bacf-60320cf8b3d5.jpg"},
    { path: "Thumbnails/2017/2017-07-12_f68e00cb-503f-4143-ad8a-1eee8be086ba.jpg"},
    { path: "Thumbnails/2017/2017-07-13_02be67bb-c1fc-4199-bc47-c37c0a251297.jpg"},
    { path: "Thumbnails/2017/2017-07-13_611152da-be99-421a-903f-a9048d22c34f.jpg"},
    { path: "Thumbnails/2017/2017-07-13_54131521-fa98-45a4-996e-2ae1e510fe37.jpg"},
    { path: "Thumbnails/2017/2017-07-13_bee1e7fe-72d7-49d0-8b09-5140b6e238d1.jpg"},
    { path: "Thumbnails/2017/2017-07-15_0bfdbf8f-5db9-4cdd-a14b-cd9f153727de.jpg"},
    { path: "Thumbnails/2017/2017-07-27_9369c147-ebda-4759-8abd-02e6d4816259.jpg"},
    { path: "Thumbnails/2017/2017-08-04_49473fad-8cab-4f23-870d-b2ce8008884a.jpg"},
    { path: "Thumbnails/2017/2017-08-24_35447ea6-0350-497d-a337-a839d77c54a7.jpg"},
    { path: "Thumbnails/2017/2017-09-12_0e52d229-eb56-467e-adfd-1e7258a9661a.jpg"},
    { path: "Thumbnails/2017/2017-09-12_3a35676a-52e8-4320-b6ce-632c12eeae38.jpg"},
    { path: "Thumbnails/2017/2017-09-19_84a75078-eff7-4eeb-90f2-65138e0710dc.jpg"},
    { path: "Thumbnails/2017/2017-09-27_62764ec9-c22e-46d2-9c41-eb6308edaa6c.jpg"},
    { path: "Thumbnails/2017/2017-11-04_2bb9bb07-e9e0-4c2f-ab79-52ae034acef1.jpg"},
    { path: "Thumbnails/2017/2017-11-04_5f430241-8514-4065-9600-0cb6ff0a0396.jpg"},
    { path: "Thumbnails/2017/2017-11-04_7d032b71-c6bc-44c6-a375-370339ca1e44.jpg"},
    { path: "Thumbnails/2017/2017-11-04_7db1eba7-14ab-47b7-a37a-74c624faa454.jpg"},
    { path: "Thumbnails/2017/2017-11-04_9a597e28-0179-425d-927f-66388d2b9051.jpg"},
    { path: "Thumbnails/2017/2017-11-04_9d6d432b-375f-4717-8ea6-1750c67f2847.jpg"},
    { path: "Thumbnails/2017/2017-11-04_13db4de9-78bd-43fa-8b84-66d2a9c2f7c7.jpg"},
    { path: "Thumbnails/2017/2017-11-04_28e0a50c-ce55-4bbe-a404-b456d0ae54b8.jpg"},
    { path: "Thumbnails/2017/2017-11-04_246e3b9d-8aa1-4e5c-a589-17c3b235f9ba.jpg"},
    { path: "Thumbnails/2017/2017-11-04_928ead99-333f-4c5d-8274-297c6e24d7c9.jpg"},
    { path: "Thumbnails/2017/2017-11-04_1355063e-41d2-4d6e-a05b-728d8edb6ecb.jpg"},
    { path: "Thumbnails/2017/2017-11-04_e5a8c21e-05e0-457c-916a-5487063c2943.jpg"},
    { path: "Thumbnails/2017/2017-11-04_e72fa5e9-7553-46ca-820d-b36efd7f1958.jpg"},
    { path: "Thumbnails/2017/2017-11-04_e57660a1-fd87-40fe-8dd0-398ef464a64e.jpg"},
    { path: "Thumbnails/2017/2017-11-04_ea4231d2-a0b4-4abb-9de0-32fc1e598500.jpg"},
    { path: "Thumbnails/2017/2017-11-04_ebc2af45-dce4-47c7-b589-92d0d00c62d4.jpg"},
    { path: "Thumbnails/2017/2017-11-04_f5bc995d-e969-40be-bb72-79848366b1ec.jpg"},
    { path: "Thumbnails/2017/2017-11-05_f5f71a86-4203-4c39-86f6-20be8a38c74b.jpg"},
    { path: "Thumbnails/2017/2017-11-09_28c735c9-d9ad-48cd-9388-90da02f52f09.jpg"},
    { path: "Thumbnails/2017/2017-11-09_37892bd9-f472-4031-9379-7833877eac7e.jpg"},
    { path: "Thumbnails/2017/2017-11-11_2e7efe85-8004-49a8-bbf5-71b03fea8a99.jpg"},
    { path: "Thumbnails/2017/2017-11-11_3556f7a3-4d9a-4142-b839-9b5e66170bcf.jpg"},
    { path: "Thumbnails/2017/2017-11-19_558c95e3-ba67-45da-a301-e77212f86ebb.jpg"},
    { path: "Thumbnails/2017/2017-11-19_6186658f-3e65-42cb-ab1a-061abc0d96db.jpg"},
    { path: "Thumbnails/2017/2017-11-24_0a84f953-b3a1-4174-8895-e78d5927868d.jpg"},
    { path: "Thumbnails/2017/2017-11-24_0b0ff588-1a3c-4057-aac7-2ae0f00a437e.jpg"},
    { path: "Thumbnails/2017/2017-11-24_05e7b427-8360-458b-a19f-01cc2056af89.jpg"},
    { path: "Thumbnails/2017/2017-11-24_5cccaf29-5cb7-47be-8583-77e5a5b526b5.jpg"},
    { path: "Thumbnails/2017/2017-11-24_4609b4cd-e9eb-469b-9c23-b68e82726e4d.jpg"},
    { path: "Thumbnails/2017/2017-11-24_a0a4f6cf-585c-48da-a7bb-5900226cb1d8.jpg"},
    { path: "Thumbnails/2017/2017-11-24_bee8aaac-a6e3-41fa-bcb6-c866dd7f8a9c.jpg"},
    { path: "Thumbnails/2017/2017-11-27_83f56b0d-10ba-4419-9904-0c8b07b2e808.jpg"},
    { path: "Thumbnails/2017/2017-11-27_546bff0a-baf6-4e82-807a-23c14f1eb94e.jpg"},
    { path: "Thumbnails/2017/2017-12-03_43ad4e1d-5f22-49ae-b42a-89c25bc23afe.jpg"},
    { path: "Thumbnails/2017/2017-12-03_348c7f75-81e1-47a1-9589-6611daa0187a.jpg"},
];

let loadedMediaItems = [];

export function init(displayElement) {    

    displayElement.style.overflow = "hidden";

    const domRect = displayElement.getBoundingClientRect();
    columnWidth = parseInt(domRect.width / visibleColumns);
    rowHeight = parseInt(domRect.height / visibleRows);

    for (let c = 0; c < visibleColumns + 1; c++) {
        for(let r = 0; r < visibleRows; r++) {
            let dataIndex = c * visibleRows + r;
            let rect = { x: c * columnWidth, y: r * rowHeight, w: columnWidth, h: rowHeight };
            let mediaItem = createMediaItem(dataIndex, rect, c, r);
            loadedMediaItems.push(mediaItem);
            displayElement.append(mediaItem.img);
        }
    }

    focusMediaItem(loadedMediaItems[0]);

    document.addEventListener('keydown', function (e) {        
        if (e.key === "ArrowRight") {
            moveCursorHorizontally(e, true);
        } else if (e.key === "ArrowLeft") {
            moveCursorHorizontally(e, false);
        }
        else if (e.key === "ArrowDown") {
            moveCursorVertically(e, true);
        } else if (e.key === "ArrowUp") {
            moveCursorVertically(e, false);
        }
    });
    // document.addEventListener('keyup', function (e) {
    //     if (e.key === "ArrowRight") {
    //         console.log("up");
    //         //moveRight(e.repeat === false);
    //     }
    // });

    initCSS(columnWidth);
}

function initCSS(columnWidthInPx) {

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
    style.innerHTML = keyFrames.replace(/A_DYNAMIC_VALUE/g, "" + columnWidthInPx);
    document.getElementsByTagName('head')[0].appendChild(style);
}

function createMediaItem(dataIndex, rect, gridColumnIndex, gridRowIndex) {

    dataIndex = dataIndex % currentYearItems.length;

    let mediaItem = {
        dataIndex: dataIndex,
        rect: rect,   
        img: document.createElement('img'),
        gridColumnIndex: gridColumnIndex,
        gridRowIndex: gridRowIndex
    };

    mediaItem.img.id = "mi_" + dataIndex;
    mediaItem.img.src = currentYearItems[dataIndex].path;
    mediaItem.img.style.position = "absolute";
    mediaItem.img.style.left = rect.x + "px";    
    mediaItem.img.style.top= rect.y + "px";    
    mediaItem.img.style.width = rect.w + "px";
    mediaItem.img.style.height = rect.h + "px";       
    mediaItem.img.mediaItem = mediaItem;

    return mediaItem;
}

function focusMediaItem(mediaItem) {

    if (focusedMediaItem != null)
        focusedMediaItem.img.classList.remove("focused-media-item");

    mediaItem.img.classList.add("focused-media-item");
    focusedMediaItem = mediaItem;
}

function moveCursorHorizontally(e, moveRight) {

    if (animationsPlaying > 0)
        return;

    if (moveRight) {
        if (focusedMediaItem.gridColumnIndex < visibleColumns - 1) {
            focusMediaItemByOffset(visibleRows);
            return;
        }
    }
    else {
        if (focusedMediaItem.gridColumnIndex > 0) {
            focusMediaItemByOffset(-visibleRows);
            return;
        }
    }

    scroll(e.repeat === false, moveRight);
    
    focusMediaItemByOffset(moveRight ? visibleRows : -visibleRows);
}

function moveCursorVertically(e, moveDown) {    
    focusMediaItemByOffset(moveDown ? 1 : -1);
}

function focusMediaItemByOffset(offset) {
    let rightNeighbor = loadedMediaItems.find(mi => mi.dataIndex == focusedMediaItem.dataIndex + offset);
    if (rightNeighbor)
        focusMediaItem(rightNeighbor);
}

function scroll(withAnimation, moveRight) {

    if (moveRight && (firstColumnOffset + visibleColumns) * 2 >= currentYearItems.length)
        return;
    
    if (!moveRight && firstColumnOffset <= 0)
        return;

    shiftLoadedMediaItemsByOneGridColumn(withAnimation, moveRight);

    loadedMediaItems = loadedMediaItems.filter(mediaItem => mediaItem.gridColumnIndex >= -1 && mediaItem.gridColumnIndex <= visibleColumns);
    
    if (moveRight) {
        firstColumnOffset++;
        appendNewItems(firstColumnOffset + visibleColumns, true);
    }
    else {
        firstColumnOffset--;    
        appendNewItems(firstColumnOffset - 1, false);
    }
}

function shiftLoadedMediaItemsByOneGridColumn(withAnimation, moveRight) {
    loadedMediaItems.forEach(mediaItem => {

        if (moveRight) {
            mediaItem.rect.x -= columnWidth;
            mediaItem.gridColumnIndex -= 1;
        }
        else {
            mediaItem.rect.x += columnWidth;
            mediaItem.gridColumnIndex += 1;
        }

        if (mediaItem.gridColumnIndex < -1 || mediaItem.gridColumnIndex > visibleColumns) {
            mediaItem.img.remove();
        }
        else {
            if (withAnimation) {
                mediaItem.img.addEventListener('animationend', moveAnimationCompleted);
                if (moveRight)
                    mediaItem.img.style.animation = "shiftLeftDyn 0.15s 1";
                else 
                    mediaItem.img.style.animation = "shiftRightDyn 0.15s 1";
                animationsPlaying++;
            }
            else {
                mediaItem.img.style.left = mediaItem.rect.x + "px";
            }
        }
    });
}

function appendNewItems(newLastVisibleColumn, addRight) {
    for (let r = 0; r < visibleRows; r++) {
        let dataIndex = newLastVisibleColumn * visibleRows + r;

        if (dataIndex < 0 || dataIndex >= currentYearItems.length)
            return;

        let loadingIsRequired = true;
        loadedMediaItems.forEach(mi => {
            if (mi.dataIndex == dataIndex) {
                loadingIsRequired = false;
                return;
            }                
        });

        if (!loadingIsRequired)
            return;

        let addToGridColumn = -1;
        if (addRight)
            addToGridColumn = visibleColumns;

        let rect = { x: addToGridColumn * columnWidth, y: r * rowHeight, w: columnWidth, h: rowHeight };
        let mediaItem = createMediaItem(dataIndex, rect, addToGridColumn, r);
        loadedMediaItems.push(mediaItem);
        displayElement.append(mediaItem.img);
    }
}

function moveAnimationCompleted(e) {    
    let mediaItem = e.srcElement.mediaItem;    
    mediaItem.img.style.left = mediaItem.rect.x + "px";
    e.srcElement.style.animation = null;

    animationsPlaying--;
}
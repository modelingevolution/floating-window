// Multi-window support - stores references by window ID
const windows = new Map();

export function initialize(windowId, dotNetRef) {
    windows.set(windowId, {
        dotNetRef: dotNetRef,
        isDragging: false,
        isResizing: false
    });
}

export function startDrag(windowId) {
    const windowData = windows.get(windowId);
    if (windowData) {
        windowData.isDragging = true;
        windowData.isResizing = false;
        setupListeners(windowId);
    }
}

export function startResize(windowId) {
    const windowData = windows.get(windowId);
    if (windowData) {
        windowData.isDragging = false;
        windowData.isResizing = true;
        setupListeners(windowId);
    }
}

function setupListeners(windowId) {
    const onMouseMove = (e) => {
        const windowData = windows.get(windowId);
        if (windowData && windowData.dotNetRef) {
            windowData.dotNetRef.invokeMethodAsync('OnMouseMove', e.clientX, e.clientY);
        }
    };

    const onMouseUp = () => {
        const windowData = windows.get(windowId);
        if (windowData) {
            windowData.isDragging = false;
            windowData.isResizing = false;
            if (windowData.dotNetRef) {
                windowData.dotNetRef.invokeMethodAsync('OnMouseUp');
            }
        }
        document.removeEventListener('mousemove', onMouseMove);
        document.removeEventListener('mouseup', onMouseUp);
    };

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
}

export function getViewportSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

export function dispose(windowId) {
    windows.delete(windowId);
}

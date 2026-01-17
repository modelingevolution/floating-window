let dotNetRef = null;

export function initialize(ref) {
    dotNetRef = ref;
}

export function startDrag() {
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
}

export function startResize() {
    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
}

function onMouseMove(e) {
    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnMouseMove', e.clientX, e.clientY);
    }
}

function onMouseUp() {
    document.removeEventListener('mousemove', onMouseMove);
    document.removeEventListener('mouseup', onMouseUp);
    if (dotNetRef) {
        dotNetRef.invokeMethodAsync('OnMouseUp');
    }
}

export function dispose() {
    document.removeEventListener('mousemove', onMouseMove);
    document.removeEventListener('mouseup', onMouseUp);
    dotNetRef = null;
}

export function toDateObjString(date: string, time: string) {
    return date + "T" + time;
}

export function addToDateObjString(date: string, minutes: number) {
    const d = new Date(date);
    d.setMinutes(d.getMinutes() + minutes);

    const offset = d.getTimezoneOffset() * 60000;
    const localDate = new Date(d.getTime() - offset);

    return localDate.toISOString().slice(0, 19);
}
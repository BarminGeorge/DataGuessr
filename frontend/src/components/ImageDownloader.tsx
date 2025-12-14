
export default async function fetchImageUrl(url: string): Promise<string> {
    if (!url)
        return "";
    const res = await fetch("https://fiitguesser.ru" + url);

    if (!res.ok) {
        throw new Error(`Image fetch failed: ${res.status}`);
    }

    const blob = await res.blob();
    console.log("fetched");
    return URL.createObjectURL(blob);
}


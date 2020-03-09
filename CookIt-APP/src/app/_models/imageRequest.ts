export class ImageRequest {
    src: string;
    width: number;
    height: number;
    constructor(srcUnencoded: string) {
        this.src = encodeURIComponent(srcUnencoded);
    }

 }

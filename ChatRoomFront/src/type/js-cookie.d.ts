declare module 'js-cookie' {
    interface Cookies {
        set(name: string, value: string, options?: Cookies.CookieAttributes): void;
        get(name: string): string | undefined;
        remove(name: string, options?: Cookies.CookieAttributes): void;
    }
    
    const Cookies: Cookies;
    export default Cookies;
}

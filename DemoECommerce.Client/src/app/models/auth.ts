export interface Response {
    flag: boolean;
    message: string;
    token?: string;
}

export interface AppUserDTO {
    id?: number;
    name: string;
    email: string;
    password?: string;
    role: string;
}

export interface LoginDTO {
    email: string;
    password?: string;
}

export interface UserSession {
    token: string;
    id: number;
    name: string;
    email: string;
    role: string;
}

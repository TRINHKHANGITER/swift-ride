export type LoginContract = {
    phone: string;
    password: string;
}

export type LoginView = {
    token: string;
    accountUuid: string;
    role: string;
    iat: number;
    exp: number;
}

export type RegisterContract = {
    fullname: string;
    phone: string;
    password: string;
}

export type RegisterView = {
    uuid: string;
    fullname: string;
    phone: string;
    role: string;
}

export type RegisterDriverContract = {
    licenseNumber: string;
    vehicleName: string;
    licensePlate: string;
}

// ! Todo later
export type RegisterDriverView = Record<string, never>;

export type AboutMeView = {
    account: AccountView;
    profile?: ProfileView;
}

type AccountView = {
    uuid: string;
    fullname: string;
    phone: string;
    role: string;
}

export type ProfileView = {
    profileUuid: string;
    licenseNumber: string;
    vehicleName: string;
    licensePlate: string;
    isVerified: boolean;
}

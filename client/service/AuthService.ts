import { httpClient } from "@/lib/http-client";
import { AboutMeView, LoginContract, LoginView, RegisterContract, RegisterDriverContract, RegisterView } from "./contract/AuthContract";

class AuthService {
    async login(contract: LoginContract): Promise<LoginView> {
        const response = await httpClient.post<LoginView>("/ts/auth/login", contract);
        return response.data;
    }

    async register(contract: RegisterContract): Promise<RegisterView> {
        const response = await httpClient.post<RegisterView>("/ts/auth/register", contract);
        return response.data;
    }

    async registerDriver(contract: RegisterDriverContract): Promise<void> {
        await httpClient.post("/ts/auth/register-driver", contract);
    }

    async getAboutMe(): Promise<AboutMeView> {
        const response = await httpClient.get<AboutMeView>("/ts/auth/about-me");
        return response.data;
    }
}

export const authService = new AuthService();
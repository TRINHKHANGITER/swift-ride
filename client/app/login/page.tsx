import { LoginForm } from "@/components/auth/LoginForm";
import { GuestOnlyScreen } from "@/components/auth/GuestOnlyScreen";

export default function LoginPage() {
  return (
    <GuestOnlyScreen>
      <LoginForm />
    </GuestOnlyScreen>
  );
}

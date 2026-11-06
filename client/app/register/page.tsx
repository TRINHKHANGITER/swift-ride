import { RegisterForm } from "@/components/auth/RegisterForm";
import { GuestOnlyScreen } from "@/components/auth/GuestOnlyScreen";

export default function RegisterPage() {
  return (
    <GuestOnlyScreen>
      <RegisterForm />
    </GuestOnlyScreen>
  );
}

import { useNavigate, useLocation } from "react-router-dom";
import { useCookies } from "react-cookie";
import {useState} from "react";

export default function LoginComponent() {
    const [, setCookie] = useCookies(["userEmail"]);
    const navigate = useNavigate();
    const location = useLocation();
    const [email, setEmail] = useState<string>("");

    const from = location.state?.from?.pathname || "/";

    const handleLogin = (email: string) => {
        setCookie("userEmail", email, { path: "/", maxAge: 86400, sameSite: "lax" });
        navigate(from, { replace: true });
    };

    return (
        <div>
            <h2>Login</h2>
            <div>
                <input
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    type="email"
                    name="email"/>
            </div>
            <button
                disabled={email === ""}
                onClick={() => handleLogin(email)}>
                Login
            </button>
        </div>
    );
}
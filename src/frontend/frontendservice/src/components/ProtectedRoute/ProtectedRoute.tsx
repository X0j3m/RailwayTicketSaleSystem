import type {ReactNode} from "react";
import { Navigate, useLocation } from "react-router-dom";
import { useCookies } from "react-cookie";

interface ProtectedRouteProps {
    children: ReactNode;
}

export function ProtectedRoute({ children }: ProtectedRouteProps) {
    const [cookies] = useCookies(["userEmail"]);
    const location = useLocation();

    if (!cookies.userEmail) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return <>{children}</>;
}
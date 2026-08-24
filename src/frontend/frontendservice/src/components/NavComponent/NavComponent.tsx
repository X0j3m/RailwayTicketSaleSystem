import {useCookies} from "react-cookie";
import {useNavigate} from "react-router-dom";
import {sendTicketsQuery} from "../../utils/UseTickets.ts";
import {useSignalR} from "../../hooks/useSignalR.ts";

function NavComponent() {
    const {connection} = useSignalR();

    const [cookies, , removeCookie] = useCookies(['userEmail']);
    const navigate = useNavigate();

    const handleLogin = () => {
        navigate("/login", {replace: true});
    }

    const handleLogout = () => {
        removeCookie("userEmail");
    }

    const handleMyTickets = () => {
        if (!connection || !cookies?.userEmail) return;

        const email = cookies?.userEmail;

        sendTicketsQuery(
            connection,
            email);

        navigate("/tickets", {replace: true});
    }

    const handleHome = () => {
        navigate("/");
    }

    return (
        <>
            <button
                onClick={handleHome}>
                Home
            </button>
            {
                cookies.userEmail
                    ?
                    <>
                        <button
                            onClick={handleMyTickets}>
                            My tickets
                        </button>
                        <button
                            onClick={handleLogout}>
                            Logout
                        </button>
                    </>
                    :
                    <button
                        onClick={handleLogin}>
                        Login
                    </button>
            }
            <br/>
            <br/>
        </>
    );
}

export default NavComponent
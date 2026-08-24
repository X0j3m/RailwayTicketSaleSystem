import { RouterProvider, createBrowserRouter, Outlet } from "react-router-dom";
import TrainScheme from "./components/TrainScheme/TrainScheme.tsx";
import { useTrainStations } from "./utils/UseTrainStations.ts";
import TrainConnections from "./components/TrainConnections/TrainConnections.tsx";
import TicketSummaryComponent from "./components/TicketSummaryComponent/TicketSummaryComponent.tsx";
import CheckoutInfoComponent from "./components/CheckoutInfoComponent/CheckoutInfoComponent.tsx";
import TicketsGridComponent from "./components/TicketsGridComponent/TicketsGridComponent.tsx";
import { ProtectedRoute } from "./components/ProtectedRoute/ProtectedRoute.tsx";
import LoginComponent from "./components/LoginComponent/LoginComponent.tsx";
import NavComponent from "./components/NavComponent/NavComponent.tsx";

function RootLayout() {
    return (
        <>
            <NavComponent />
            <main>
                <Outlet />
            </main>
        </>
    );
}

export function App() {
    const trainStations = useTrainStations();

    const router = createBrowserRouter([
        {
            path: "/",
            element: <RootLayout />,
            children: [
                {
                    index: true,
                    element: <TrainConnections trainStations={trainStations} />
                },
                {
                    path: "login",
                    element: <LoginComponent />
                },
                {
                    path: "seat-selection",
                    element: <TrainScheme trainStations={trainStations} />
                },
                {
                    path: "ticket-summary",
                    element: (
                        <ProtectedRoute>
                            <TicketSummaryComponent trainStations={trainStations} />
                        </ProtectedRoute>
                    )
                },
                {
                    path: "checkout-info",
                    element: (
                        <ProtectedRoute>
                            <CheckoutInfoComponent />
                        </ProtectedRoute>
                    )
                },
                {
                    path: "tickets",
                    element: (
                        <ProtectedRoute>
                            <TicketsGridComponent trainStations={trainStations} />
                        </ProtectedRoute>
                    )
                }
            ]
        }
    ]);

    return <RouterProvider router={router} />;
}

export default App;
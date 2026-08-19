import {RouterProvider} from 'react-router-dom';
import {createBrowserRouter} from "react-router-dom";
import TrainScheme from "./components/TrainScheme/TrainScheme.tsx";
import {useTrainStations} from "./utils/UseTrainStations.ts";
import TrainConnections from "./components/TrainConnections/TrainConnections.tsx";
import TicketSummaryComponent from "./components/TicketSummaryComponent/TicketSummaryComponent.tsx";

export function App() {
    const trainStations = useTrainStations();

    const router = createBrowserRouter([
        {
            path: "/",
            element: <TrainConnections trainStations={trainStations}/>
        },
        {
            path: "/seat-selection",
            element: <TrainScheme trainStations={trainStations}/>
        },
        {
            path: "/ticket-summary",
            element: <TicketSummaryComponent trainStations={trainStations}/>
        }
    ]);

    return (
        <RouterProvider router={router}/>
    );
}

export default App

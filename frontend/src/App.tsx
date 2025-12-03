// import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import LoginPage from "./pages/LoginPage";
import RegistrationPage from "./pages/RegistrationPage";

import { usePage } from "./PageContext";
import ProfilePage from "./pages/ProfilePage";

// export default function App() {
//   return (
//     <BrowserRouter>
//       <Routes>
//         <Route path="/" element={<Home />} />
//         <Route path="/login" element={<LoginPage />} />
//         <Route path="/register" element={<RegistrationPage />} />
//       </Routes>
//     </BrowserRouter>
//   );
// }

export default function App() {
  const { page, setPage } = usePage();

  return (
    <div>
      {page === "home" && <Home />}
      {page === "login" && <LoginPage />}
      {page === "registration" && <RegistrationPage />}
      {page === "profile" && <ProfilePage />}
    </div>
  );
}


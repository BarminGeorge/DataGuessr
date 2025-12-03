import React from "react";
import '../App.css';
import { usePage } from "../PageContext";

export default function Header({variant  = "logo"}) {
  if (variant === "logo-and-login-button") {
    return (
      <HeaderWithLogoAndLoginButton />
    );
  } 
  else {
  return (
      <HeaderWithOnlyLogo />
  );
}
}

function HeaderWithOnlyLogo() {
const { setPage } = usePage();
  return (
    <div className="header-container">
      <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>

    </div>
  );
}

function HeaderWithLogoAndLoginButton() {
const { setPage } = usePage();
  return (
    <div className="header-container">
      <div className="title-text" onClick={() => setPage("home")}>FIITguesser</div>

      <div className="flex items-center gap-3">
        <button className="button-primary" onClick={() => setPage("login")}>Login</button>
        <img onClick={() => setPage("profile")}
          src="src/assets/defaultavatar.jpg"
          alt="avatar"
          className="w-10 h-10 rounded-full border border-gray-300"
        />
      </div>
    </div>
  );
}

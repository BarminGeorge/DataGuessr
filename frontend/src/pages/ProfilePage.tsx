import React from "react";
import Header from "../components/Header";


import { usePage } from "../PageContext";
export default function ProfilePage() {
  const { setPage } = usePage();
  return (
    <div className="global-container"> 
    <Header />
      <div className="main-container">  
        <div className="title-text">  
        Параметры профиля
        </div>

       
        <button className="button-primary" onClick={() => setPage("home")}>Сохранить</button>;

    </div>
    </div>
  );
}
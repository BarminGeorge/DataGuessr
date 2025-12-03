import React from "react";
import Header from "../components/Header";
import { http } from "../api/http";

import { useState } from "react";
import { usePage } from "../PageContext";
import { validateLogin, validatePassword, validateUsername } from "../utils/validations";

async function handleRegistration(
  login: string, 
  password: string, 
  playerName: string,
  avatar: any,
  setPage: (page:any) => void) {
  
  try {
    const res: any = await http.post(
      "/register", 
      { login, password, playerName, avatar }, 
      {headers: {
      "Content-Type": "multipart/form-data",
    }})


    localStorage.setItem("token", res.token);
    setPage("home");
    } catch (e) {
      alert(e);
    }
}

export default function RegistrationPage() {
  const { setPage } = usePage();
  const [login, setLogin] = useState("");
  const [password, setPassword] = useState("");
  const [playerName, setPlayerName] = useState("");
  const [avatar, setAvatar] = useState<File | null>(null);
  

  return (
    <div className="global-container">
      <Header />
      <div className="container">  
        <div className="title-text">  
        Регистрация
        </div>


        <div className="secondary-container">
        <input 
          type="text"
          className="text-input-primary"
          placeholder={"Придумайте логин"}
          onChange={(e) => setLogin(e.target.value)}
          />
          <span className="acсent-text">
            {validateLogin(login)}
          </span>
        </div>
        <div className="secondary-container">
        <input 
          type="text"
          className="text-input-primary"
          placeholder={"Придумайте пароль"}
          onChange={(e) => setPassword(e.target.value)}/>
        <span className="acсent-text">
                    {validatePassword(password)}
                  </span>
        </div>
        <div className="secondary-container">
        <input 
          type="text"
          className="text-input-primary"
          placeholder={"Придумайте имя пользователя"}
          onChange={(e) => setPlayerName(e.target.value)}/>
        <span className="acсent-text">
          {validateUsername(playerName)}
        </span>
        </div>

        <input 
          type="file"
          className="text-input-primary"
          placeholder={"Выберите аватар"}
          onChange={(e) => setAvatar(e.target.files && e.target.files[0] ? e.target.files[0] : null)}/>

        
        <button className="button-primary" 
        onClick={() => handleRegistration(login, password, playerName, avatar, setPage)}>Зарегистрироваться</button>;

    </div>
    </div>
  );
}




import React from "react";
import Header from "../components/Header";
import TextInput from "../components/TextInputs";

import { usePage } from "../PageContext";
import { http } from "../api/http";
import { useState } from "react";
import { validateLogin, validatePassword } from "../utils/validations";

async function handleLogin(
  login: string, 
  password: string,
  setPage: (page:any) => void) {
  
  try {
    const res: any = await http.post(
      "/login", 
      {login, password},
      {})

    if (res.success === true) {
      setPage("home");
    } else {
      alert("Упс, что-то пошло не так");
    }
    } catch (e) {
      errorParser(e);
    }
}


function errorParser(e : any) {
  console.log(e);
  return
}


export default function LoginPage() {
   const { setPage } = usePage();
   const [login, setLogin] = useState("");
   const [password, setPassword] = useState("");

  return (
    <div className="global-container">
      <Header />
      <div className="main-container">  
        <div className="title-text">  
        Вход
        </div>

        <div className="secondary-container">
          <input 
            type="text"
            className="text-input-primary"
            placeholder={"Введите логин"}
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
            placeholder={"Введите пароль"}
            onChange={(e) => setPassword(e.target.value)}/>
          <span className="acсent-text">
            {validatePassword(password)}
          </span>
          
        </div>
        <div className="secondary-text">Нет аккаунта? 
        
        <span 
          className="link-text"
          onClick={() => setPage("registration")}>
          Зарегистрироваться
        </span>
  
        </div>
       
        <button className="button-primary" 
          onClick={() => handleLogin(login, password, setPage)}>
            Войти
        </button>

    </div>
    </div>
  );
}
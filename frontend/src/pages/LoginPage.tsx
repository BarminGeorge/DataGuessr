import React from "react";
import Header from "../components/Header";
import TextInput from "../components/TextInputs";
import { usePage } from "../PageContext";
import { http } from "../api/http";
import { useState } from "react";

async function handleLogin(
  login: string, 
  password: string,
  setPage: (page:any) => void) {
  
  try {
    const res: any = await http.post(
      "/login", 
      {login, password }, 
      {headers: {
      "Content-Type": "application/json",
    }})

    localStorage.setItem("token", res.token);
    setPage("home");
    } catch (e) {
      alert(e);
    }
}


export default function LoginPage() {
   const { setPage } = usePage();
   const [login, setLogin] = useState("");
   const [password, setPassword] = useState("");

  return (
    <div className="global-container">
      <Header />
      <div className="container">  
        <div className="title-text">  
        Вход
        </div>
        <input 
          type="text"
          className="text-input-primary"
          placeholder={"Придумайте логин"}
          onChange={(e) => setLogin(e.target.value)}
          />
        <input 
          type="text"
          className="text-input-primary"
          placeholder={"Придумайте пароль"}
          onChange={(e) => setPassword(e.target.value)}/>
        <div className="secondary-text">Нет аккаунта? 
          <span className="link-text" onClick={() => setPage("registration")}>Зарегистрироваться</span></div>
       
        <button className="button-primary" 
        onClick={() => handleLogin(login, password, setPage)}>Войти</button>;

    </div>
    </div>
  );
}
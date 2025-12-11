import React from "react";
import Header from "../components/Header";
import TextInput from "../components/TextInputs";
import http from "../api/http";
import { usePage } from "../PageContext";
import EnterModal from "../modals/EnterModal";

async function RandomRoom(
  setPage: (page:any) => void) {
  
  try {
    const res: any = await http.get(
      "/rooms");
    
    setPage("room");
    } catch (e) {
      alert(e);
      setPage("room");
    }
}

function checkLogging() {
  return false;
}

export default function HomePage() {
  const { setPage } = usePage();

  return (
    
    <div className="global-container">
      
      {/* <button className="button-primary" onClick={() => setPage("game")}>В Игре</button>;   */}
      <div className={`modal centered-vertical-aligment ${checkLogging() ? 'hide': ''}`}>
      <EnterModal />
      </div>
      <div className={checkLogging() ? "": "blur"}>


      <Header variant={checkLogging()? "logo-and-avatar-and-create-room" : "logo-and-login-button"}/>
      <div className="main-container">      
      <div className="secondary-container">
        <div className="left-aligment title-variant-1">Время</div>
        <div className="centered-aligment title-variant-2">
            Угадывать
        </div>
        <div className="right-aligment  title-variant-3">Время</div>
        </div>

    <div className="right-aligment">     
      <p className="secondary-text max-w-xl">
        Это мега крутая игра где тебе нужно по фотографии угадать время когда
        она сделана, на всё про всё несколько минут, угадал лучше всех —
        победил!
      </p>
    </div>

    {/* join game buttons */}
    <div className="secondary-container">
      <div className="right-aligment">
        <TextInput Text="Введите код приглашения" />

        <button className="button-primary">Присоединиться</button>
      
        <span className="mx-1 text-gray-600">или</span>

        <button className="button-primary" onClick={() => RandomRoom(setPage)}>Случайная игра</button>;
      </div>
    </div>
    </div>
    </div>
    </div>
  );
}

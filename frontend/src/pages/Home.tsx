import React from "react";
import Header from "../components/Header";
import TextInput from "../components/TextInputs";

export default function HomePage() {
  return (
    <div className="global-container">
      <Header variant="logo-and-login-button" />
      <div className="container">      
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

        <button className="button-primary">Присоединиться</button>;

        <span className="mx-1 text-gray-600">или</span>

        <button className="button-primary">Случайная игра</button>;
      </div>
    </div>
    </div>
    </div>
  );
}

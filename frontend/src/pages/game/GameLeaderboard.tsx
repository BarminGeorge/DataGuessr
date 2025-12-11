import React from "react";
import Header from "../../components/Header";
import PlayerCard from "../../components/Cards";

export default function GameLeaderboard() {
  const playerList = [];
  for (let i = 0; i < 10; i++) {
    playerList.push(<PlayerCard variant="score" username="Имя пользователя" score={(100 - i*10) ** 2} />);
  }


  return (
    <div className="global-container">
      <Header variant="logo-and-timer"/>
      <div className="main-container">  

      <div className="row-template">  
      <div className="down-picture row-column">
        <div className="down-element">
          <img
          src="src/assets/defaultavatar.jpg"
          className="default-picture"
        />
        </div>
      </div>
      <div className="list-container row-column">
        <div className="title-text-2">
        Лидеры
        </div>
        {playerList}
      </div>
      <div className="down-picture row-column">
        <div className="up-element">
        <img
          src="src/assets/defaultavatar.jpg"
          className="default-picture"
        />
        </div>
      </div>
    </div>
    </div>
    </div>
  );
}
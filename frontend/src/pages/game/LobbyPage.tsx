import FormControl from '@mui/material/FormControl';
import FormControlLabel from '@mui/material/FormControlLabel';
import FormLabel from '@mui/material/FormLabel';
import Radio from '@mui/material/Radio';
import RadioGroup from '@mui/material/RadioGroup';
import PlayerCard from "../../components/Cards";
import Header from "../../components/Header";
import { gameHubService } from '../../apiUtils/HubServices';
import { RoomPrivacy, type PlayerDto, GameMode } from '../../apiUtils/dto';
import { usePage } from '../../PageContext';
import type { CurrentAppState } from '../../App';
import { leaveRoom, createGame } from '../../utils/RoomHubUtils';
import { useState } from 'react';
import defaultAvatarImg from "../../assets/defaultavatar.png";

function BpRadio(props: any) {
    return (
        <Radio
            disableRipple
            color="default"
            sx={{
                '& .MuiSvgIcon-root': {
                    fontSize: "1.5rem",
                },
            }}
            {...props}
        />
    );
}

function CustomizedRadiosGameMode(props: any) {
    return (
        <FormControl>
            <FormLabel id="game-mode-radios">Выберите режим игры</FormLabel>
            <RadioGroup
                defaultValue="0"
                aria-labelledby="game-mode-radios"
                name="customized-radios"
                onChange={(_, value) => props.setMode(Number(value))}
            >
                <FormControlLabel className="accent-text" value="0" control={<BpRadio />} label="Угадайте время" />
                <FormControlLabel className="accent-text" value="1" control={<BpRadio />} label="Правда или нет" />
                 </RadioGroup>
        </FormControl>
    );
}


function CustomizedRadiosGameTime(props: any) {
    return (
        <FormControl>
            <FormLabel id="game-time-radios">Выберите время раунда</FormLabel>
            <RadioGroup
                defaultValue="90"
                aria-labelledby="game-time-radios"
                name="customized-radios"
                onChange={(_, value) => props.setDuration(Number(value))}
            >
                <FormControlLabel className="accent-text" value="10" control={<BpRadio />} label="10 секунд" />
                <FormControlLabel className="accent-text" value="30" control={<BpRadio />} label="30 секунд" />
                <FormControlLabel className="accent-text" value="90" control={<BpRadio />} label="90 секунд" />
            </RadioGroup>
        </FormControl>
    );
}




export default function LobbyPage(props: CurrentAppState) {
    const { setPage } = usePage();
    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }
    if (props.room.ownerId === props.user?.id) {
        return (<LobbyPageCreatorView {...props} />);
    } else {
        return (<LobbyPageGuestView {...props} />);
    }

}
function LobbyPageCreatorView(props: CurrentAppState) {
    const { setPage } = usePage();
    const [mode, setMode] = useState(GameMode.Default);
    const [duration, setDuration] = useState(10);

    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }

    const playerList = props.room.players.map(
        player =>
            <PlayerCard variant="lobby-creator" username={player.playerName} avatar={player.avatarUrl} action={() => alert("Игрок кикнут")}
            />);


    return (
        <div className="global-container">
            <Header variant="logo-and-avatar-and-interactive"
                interact_action={() => leaveRoom(props.user?.id || "", props.room?.id, setPage, props.setRoom)}
                interact_label={"Leave lobby"}
                playerName={props.user?.playerName}
                avatarUrl={props.user?.avatarUrl}
            />
            <div className="main-container">

                <div className="row-template">
                    <div className="list-container row-column">
                        <div className="title-text-2">
                            Игроки
                        </div>
                        {playerList}
                    </div>


                    <div className="down-picture row-column">
                        <div className="centered-aligment">
                            <div className="title-text-2">{props.room.inviteCode}</div>
                            <div className="settings-container">
                                <div className="settings-element">
                                    <CustomizedRadiosGameMode setMode={setMode} />
                                </div>
                                <div className="settings-element">
                                    <CustomizedRadiosGameTime setDuration={setDuration} />
                                </div>
                            </div>
                            <button className="button-primary"
                                onClick={() => createGame(props.user?.id, props.room?.id, mode, duration, setPage)}>Начать</button>
                        </div>
                    </div>

                </div>
            </div>
        </div>
    );
}



function LobbyPageGuestView(props: CurrentAppState) {
    const { setPage } = usePage();

    if (props.room == null) {
        alert("undefined error");
        setPage("home");
        return;
    }
    const playerList = props.room.players.map(player => <PlayerCard variant="lobby-common" username={player.playerName} avatar={player.avatarUrl} />);

    return (
        <div className="global-container">
            <Header variant="logo-and-avatar-and-interactive"
                interact_action={() => leaveRoom(props.user?.id, props.room?.id, setPage, props.setRoom)}
                interact_label={"Leave lobby"}
            />
            <div className="main-container">

                <div className="row-template">
                    <div className="list-container row-column">
                        <div className="title-text-2">
                            Игроки
                        </div>
                        {playerList}
                    </div>


                    <div className="down-picture row-column">
                        <div className="centered-aligment">
                            <div className="title-text-2">{props.room.inviteCode}</div>
                            <div className="settings-container">
                                <div className="picture-xl-container">
                                    <img
                                        src={defaultAvatarImg}
                                        className="default-picture"
                                    />
                                </div>
                                <button className="button-primary"
                                    disabled
                                    onClick={() => alert("Начали")}>Ждём начала</button>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    );
}
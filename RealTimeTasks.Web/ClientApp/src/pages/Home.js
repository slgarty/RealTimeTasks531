import axios from 'axios';
import React, { useState, useRef, useEffect } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';
import { useAuthContext } from '../AuthContext';

const Home = () => {
    const connectionRef = useRef(null);
    const [task, setTask] = useState('');
    const [tasks, setTasks] = useState([]);
    const { user } = useAuthContext();  
    const connection = connectionRef.current;


    useEffect(() => {
        const connectHub = async () => {
            const connection = new HubConnectionBuilder()
                .withUrl("/tasks").build();
            await connection.start();
            connection.on('RenderTasks', tasks => setTasks(tasks));
            connection.invoke("GetAllTasks");
            connectionRef.current = connection;
        }

        connectHub();
    }, []);

    const onAddClick = async () => {
        await connection.invoke("newTask", task);
        setTask('');
    }

    const onDoingClick = id => {
        connection.invoke("setDoing", id);
    }

    const onCompletedClick = id => {
        connection.invoke("setDone", id);
    }

    const getButton = (userId, task) => {
        if (task.handledBy && task.handledBy === userId) {
            return <button className='btn btn-success' onClick={() => onCompletedClick(task.id)}>I'm Done</button>;
        }
        if (task.handledBy) {
            return <button className='btn btn-warning' disabled={true}>{task.userDoingIt} is doing this</button>;
        }
        return <button className='btn btn-info doing' onClick={() => onDoingClick(task.id)}>I'm Doing This one!</button>
    }


    return (
        <div className="container" style={{ marginTop: 60 }}>
            <div style={{ marginTop: 70}}>
                <div className="row">
                    <div className="col-md-10">
                        <input type="text" className="form-control" value={task} placeholder="Task Title" onChange={e => setTask(e.target.value)}/>
                    </div>
                    <div className="col-md-2">
                        <button className="btn btn-primary btn-block" onClick={onAddClick }>Add Task</button>
                    </div>
                </div>
                <table className="table table-hover table-striped table-bordered mt-3">
                    <thead>
                        <tr>
                            <th>Title</th>
                            <th>Status</th>
                        </tr></thead>
                    <tbody>
                        {tasks.map(task => {
                            return <tr key={task.id}>
                                <td>{task.title}</td>
                                <td>{getButton(user.id, task)}</td>
                            </tr>
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    )
}
export default Home;
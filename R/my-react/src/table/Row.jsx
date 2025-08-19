let counter = 0;

export function Row({text}) {
    counter++;
    return (
        <tr>
            <td>{counter}</td>
            <td>{text}</td>
        </tr>
    );
}
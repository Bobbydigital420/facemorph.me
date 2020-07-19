module App

open Elmish
open Elmish.React
open Fable.MaterialUI.Icons
open Fable.React
// open Fable.React.Props
open Feliz
open Feliz.MaterialUI
open Feliz.prop
open Feliz.Router
open Fulma
open Utils

let videoDim = 512
let imgDim = 300

type State = {
    LeftValue : string
    RightValue : string
    VidValues : (string * string) option
}

let parseUrl (path, query) =
    let fromValue = Map.tryFind "from_value" query
    let toValue = Map.tryFind "to_value" query

    {
        LeftValue = Option.defaultValue "hello" fromValue
        RightValue = Option.defaultValue ("Face of the day: " + System.DateTime.Today.ToString("yyyy/MM/dd")) toValue
        VidValues = Option.map2 (fun a b -> a,b) fromValue toValue
    }

type Msg =
    | SetLeftValue of string
    | SetRightValue of string
    | UrlChanged of (string list * Map<string, string>)
    | MakeVid

let imgSrc (dim:int) value =
    sprintf "https://api.checkface.ml/api/face/?dim=%i&value=%s" dim (encodeUriComponent value)


let vidSrc (dim:int) (fromValue, toValue) =
    sprintf "https://api.checkface.ml/api/mp4/?dim=%i&from_value=%s&to_value=%s" dim (encodeUriComponent fromValue) (encodeUriComponent toValue)

let getCurrentPath _ =
    let pathName = Browser.Dom.window.location.pathname
    let queryString = Browser.Dom.window.location.search

    let urlSegments = Router.urlSegments pathName RouteMode.Path
    let urlParams =
        (Router.createUrlSearchParams queryString).entries()
        |> Seq.map (fun arr -> (arr.[0], arr.[1]))
        |> Map.ofSeq
    urlSegments, urlParams

let init() = parseUrl (getCurrentPath()), Cmd.none

let update msg state : State * Cmd<Msg> =
    match msg with
    | SetLeftValue value ->
        { state with LeftValue = value }, Cmd.none
    | SetRightValue value ->
        { state with RightValue = value }, Cmd.none
    | MakeVid ->
        state, Cmd.navigatePath("/", ["from_value", state.LeftValue; "to_value", state.RightValue])
    | UrlChanged (path, query) ->
        parseUrl (path, query), Cmd.none

let renderSetpoint autoFocus value (onChange: string -> unit) =
    Column.column [ ] [
        Html.img [
            prop.src (imgSrc imgDim value)
            prop.width imgDim
            prop.height imgDim
        ]
        Mui.textField [
            textField.value value
            textField.onChange onChange
            textField.fullWidth true
            textField.placeholder "Just type anything"
            textField.autoFocus autoFocus
            textField.inputProps [
                prop.style [ style.textAlign.center ]
            ]
        ]
    ]

let renderContent (state:State) (dispatch: Msg -> unit) =
    Html.form [
        prop.onSubmit (fun e -> e.preventDefault(); dispatch MakeVid)
        prop.children [
            Mui.container [
                Columns.columns [ ] [
                    renderSetpoint true state.LeftValue (SetLeftValue >> dispatch)
                    renderSetpoint false state.RightValue (SetRightValue >> dispatch)
                ]
                
                Column.column [ ] [
                    Mui.button [
                        button.children "Morph"
                        button.type'.submit
                        button.color.primary
                        button.variant.contained
                    ]
                ]
            ]

            Html.div [
                match state.VidValues with
                | None -> ()
                | Some values ->
                    Feliz.Html.video [
                        prop.src (vidSrc videoDim values)
                        prop.controls true
                        prop.autoPlay true
                        prop.loop true
                        prop.muted true
                        prop.style [
                            style.display.block
                            style.margin.auto
                        ]
                        prop.poster (imgSrc imgDim (fst values)) //imgDim is already in cache
                        prop.width videoDim
                        prop.height videoDim
                    ]
            ]
        ]
    ]

let render (state:State) (dispatch: Msg -> unit) =
    React.router [
        router.pathMode
        router.onUrlChanged (getCurrentPath >> UrlChanged >> dispatch)
        router.children [
            Html.div [
                Column.column [ ] [
                    Heading.h1 [ ] [ str "morphdev" ]

                    Heading.h3 [ Heading.IsSubtitle ] [
                        str "morph with "
                        Html.a [
                           prop.children (str "checkface")
                           prop.href "https://checkface.ml"
                        ]
                        str " values"
                    ]
                ]
            ]
            renderContent state dispatch
        ]
    ]



#if DEBUG
printfn "Enabled HMR"
open Elmish.HMR
#endif

Program.mkProgram init update render
|> Program.withReactSynchronous "elmish-app"
|> Program.run
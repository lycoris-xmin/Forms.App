window.onload = function () {
  let isRunApp = false
  const label = document.querySelector('.title')
  //
  setInterval(timer, 1000)

  async function timer() {
    //
    if (!window.external || !window.external.launch) {
      return
    }

    if (!isRunApp) {
      window.external.launch.run()
      isRunApp = true
      return
    }

    let res = await window.external.launch.getLaunchStep()
    if (res) {
      label.innerText = res.stepDescription

      if (res.process >= 100) {
        window.external.root.gotoMainPage()
      }
    }
  }
}

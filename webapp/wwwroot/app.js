const file=document.querySelector('#file'),run=document.querySelector('#run'),msg=document.querySelector('#msg'),results=document.querySelector('#results');
run.onclick=async()=>{if(!file.files[0]){msg.innerHTML='<p class="error">Please select your Reliance CSV.</p>';return}
msg.textContent='Training model and evaluating the historical test period...';results.innerHTML='';
try{const csv=await file.files[0].text();const r=await fetch('/api/predict',{method:'POST',headers:{'Content-Type':'application/json'},body:JSON.stringify({csv})});const data=await r.json();if(!r.ok)throw new Error(data.message||'Prediction failed');
const cls=data.direction==='UP'?'up':'down';
results.innerHTML=`<div class="results">
<div class="box">Historical rows<div class="metric">${data.rows.toLocaleString()}</div></div>
<div class="box">Latest Close<div class="metric">${data.latestClose.toFixed(2)}</div></div>
<div class="box">Predicted Next Close<div class="metric">${data.predictedClose.toFixed(2)}</div></div>
<div class="box">Direction<div class="metric ${cls}">${data.direction}</div></div>
<div class="box">MAE<div class="metric">${data.mae.toFixed(2)}</div></div>
<div class="box">RMSE<div class="metric">${data.rmse.toFixed(2)}</div></div>
<div class="box">R²<div class="metric">${data.r2.toFixed(3)}</div></div>
<div class="box">Directional Accuracy<div class="metric">${(data.directionalAccuracy*100).toFixed(2)}%</div></div>
</div>`;
msg.innerHTML='<p class="success">'+data.message+'</p>';
}catch(e){msg.innerHTML='<p class="error">'+e.message+'</p>'}};

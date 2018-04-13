using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorCarro : MonoBehaviour {

    public Transform Tracado;
    public Transform[] Linhas;
    public int LinhaAtual = 0;
    public WheelCollider RodaFE;
    public WheelCollider RodaFD;
    public int EstercamentoMax = 40;
    public float Acelerar = 0f;
    public float Torque;
    public float Cavalos;
    public float TorqueFreio = 3000f;
    public float VelocidadeKMH = 0f;
    public float RPM;
    public int MaxRPM;
    public int MinRPM;
    public int RedLineRPM;
    public int MarchaAtual = 0;
    public int[] Marcha;
    public int TamanhoMarcha;
    public float TempoTrocaMarcha;
    public float SomPitch = 4500f;
    public bool MarchaAutomatica = true;
    public bool MarchaManual = false;
    public bool EmTroca = false;
    public bool InTrocaMais = false;
    public bool BoaMarcha = false;

    Rigidbody rb;

    public AudioClip SomCarro;
    public AudioSource AudioCarro;
    public Transform CentroMassa;
    private List<Transform> Pontos;
    private int PontoAtual = 0;

	private void Start ()
    {
        rb = GetComponent<Rigidbody>();
        AudioCarro.clip = SomCarro;
        rb.centerOfMass = CentroMassa.position;
        Torque = Cavalos * 10;
    }

    void FixedUpdate()
    {
        //Debug Log

        //Auto Direção
        Virar();
        ChecarDistancia();
        TracadoAtual();

        //Traçado
        Transform[] TracadoTransforms = Tracado.GetComponentsInChildren<Transform>();
        Pontos = new List<Transform>();

        for (int i = 0; i < TracadoTransforms.Length; i++)
        {
            if (TracadoTransforms[i] != Tracado.transform)
            {
                Pontos.Add(TracadoTransforms[i]);
            }
        }

        //Aceleração e Frenagem
        Acelerar = Input.GetAxis("Vertical");
        rb.AddForce(transform.forward * (Torque / MarchaAtual + Torque / 3f) * Acelerar);
        if (Acelerar < -0.5f)
        {
            rb.AddForce(-transform.forward * -TorqueFreio * Acelerar);
        }
        RodaFD.motorTorque = 1f;
        RodaFE.motorTorque = 1f;

        //Velocidade e RPM
        VelocidadeKMH = rb.velocity.magnitude * 3.6f;
        if (MarchaAtual > 0)
        {
            RPM = VelocidadeKMH * Marcha[MarchaAtual] * TamanhoMarcha;
        }
        if (MarchaAtual == 0)
        {
            RPM = MaxRPM * Acelerar;
        }
        AudioCarro.pitch = (RPM + 800) / SomPitch;

        //Limitador de RPM e velocidade
        if (RPM > MaxRPM)
        {
            Torque = 0;
        }
        if (RPM < MaxRPM && EmTroca == false)
        {
            Torque = Cavalos * 10;
        }

        //Marchas
        if (MarchaAtual == 0)
        {
            Torque = 0;
        }
        if (MarchaAtual < 0)
        {
            MarchaAtual++;
        }
        if (MarchaAtual == Marcha.Length - 1)
        {
            MarchaAtual--;
        }

        //Marcha Automatica
        if (MarchaAutomatica == true)
        {

        }

        //Marcha Manual
        if (MarchaManual == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Torque = 0;
                EmTroca = true;
                Invoke("RetomarMarchaMais", TempoTrocaMarcha);
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Torque = 0;
                EmTroca = true;
                Invoke("RetomarMarchaMenos", TempoTrocaMarcha);
            }
        }

    }

    void RetomarMarchaMais()
    {
        Torque = Cavalos * 10;
        MarchaAtual++;
        EmTroca = false;
    }
    void RetomarMarchaMenos()
    {
        Torque = Cavalos * 10;
        MarchaAtual--;
        EmTroca = false;
    }

    //Auto Direção
    private void Virar()
    {
        Vector3 relativevector = transform.InverseTransformPoint(Pontos[PontoAtual].position);
        float newSteer = (relativevector.x / relativevector.magnitude) * EstercamentoMax;
        RodaFD.steerAngle = newSteer;
        RodaFE.steerAngle = newSteer;
    }

    private void ChecarDistancia()
    {
        if (Vector3.Distance(transform.position, Pontos[PontoAtual].position) < 0.5f)
        { 
            if (PontoAtual == Pontos.Count - 1)
            {
                PontoAtual = 0;
            }
            else
            {
                PontoAtual++;
            }
        }
    }

    private void TracadoAtual()
    {
        Tracado = Linhas[LinhaAtual];
        if (Input.GetKeyDown(KeyCode.D))
        {
            LinhaAtual--;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            LinhaAtual++;
        }
        if (LinhaAtual > Linhas.Length)
        {
            LinhaAtual--;
        }
        if (LinhaAtual < 0)
        {
            LinhaAtual++;
        }
    }

}

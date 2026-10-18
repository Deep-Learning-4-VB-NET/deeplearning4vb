Imports System
Imports System.Reflection
Imports System.Threading
Imports RandomT = DeepLearning4VB.org.nd4vb.linalg.api.rng.Random

Namespace org.nd4vb.linalg.factory

    ''' <summary>
    ''' This class acts as factory for new Random objects and thread-isolated holder for previously created Random
    ''' instances. The random implementation is created by reflection (Java: <c>Class.newInstance()</c>); the
    ''' JavaCPP state-pointer checks of the Java class are gone with <c>Random.getStatePointer()</c>.
    ''' </summary>
    Public Class RandomFactory

        Private ReadOnly threadRandom As New ThreadLocal(Of RandomT)()
        Private ReadOnly randomClass As Type

        ''' <param name="randomClass"> a type implementing <see cref="RandomT"/> with a parameterless constructor and a
        ''' (seed As Long, size As Long) constructor </param>
        Public Sub New(ByVal randomClass As Type)
            Me.randomClass = randomClass
        End Sub

        ''' <summary>
        ''' This method returns Random implementation instance associated with calling thread
        ''' </summary>
        ''' <returns> object implementing Random interface </returns>
        Public Overridable ReadOnly Property Random As RandomT
            Get
                Try
                    If threadRandom.Value Is Nothing Then
                        Dim t As RandomT = DirectCast(Activator.CreateInstance(randomClass), RandomT)
                        threadRandom.Value = t
                        Return t
                    End If
                    Return threadRandom.Value
                Catch e As System.Exception
                    Throw New InvalidOperationException(e.Message, e)
                End Try
            End Get
        End Property

        ''' <summary>Java <c>getRandom()</c>.</summary>
        Public Overridable Function getRandom() As RandomT
            Return Random
        End Function

        ''' <summary>
        ''' This method returns new onject implementing Random interface, initialized with System.currentTimeMillis() as seed
        ''' </summary>
        ''' <returns> object implementing Random interface </returns>
        Public Overridable ReadOnly Property NewRandomInstance As RandomT
            Get
                Return getNewRandomInstance(JavaSystem.currentTimeMillis())
            End Get
        End Property

        ''' <summary>
        ''' This method returns new onject implementing Random interface, initialized with seed value
        ''' </summary>
        ''' <param name="seed"> seed for this rng object </param>
        ''' <returns> object implementing Random interface </returns>
        Public Overridable Function getNewRandomInstance(ByVal seed As Long) As RandomT
            Try
                Dim t As RandomT = DirectCast(Activator.CreateInstance(randomClass), RandomT)
                t.setSeed(seed)
                Return t
            Catch e As System.Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

        ''' <summary>
        ''' This method returns a new object implementing <see cref="RandomT"/>
        ''' interface, initialized with seed value, with size of elements in buffer
        ''' </summary>
        ''' <param name="seed"> rng seed </param>
        ''' <param name="size"> size of underlying buffer </param>
        ''' <returns> object implementing Random interface </returns>
        Public Overridable Function getNewRandomInstance(ByVal seed As Long, ByVal size As Long) As RandomT
            Try
                Dim constructor As ConstructorInfo = randomClass.GetConstructor(New Type() {GetType(Long), GetType(Long)})
                If constructor Is Nothing Then
                    Throw New MissingMethodException(randomClass.FullName & " has no (Long, Long) constructor")
                End If
                Return DirectCast(constructor.Invoke(New Object() {seed, size}), RandomT)
            Catch e As System.Exception
                Throw New InvalidOperationException(e.Message, e)
            End Try
        End Function

    End Class

End Namespace

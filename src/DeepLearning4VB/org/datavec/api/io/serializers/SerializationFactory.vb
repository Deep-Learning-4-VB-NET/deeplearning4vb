'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
'

Imports System
Imports System.Collections.Generic
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports Configured = DeepLearning4VB.org.datavec.api.conf.Configured
#Disable Warning BC40000 ' ReflectionUtils is marked obsolete, as @Deprecated in Java
Imports ReflectionUtils = DeepLearning4VB.org.datavec.api.util.ReflectionUtils
#Enable Warning BC40000
Imports Logger = DeepLearning4VB.org.slf4j.Logger
Imports LoggerFactory = DeepLearning4VB.org.slf4j.LoggerFactory

Namespace org.datavec.api.io.serializers

    ''' <summary>
    ''' Finds a <see cref="Serialization(Of T)"/> for a class among those named by the
    ''' <c>io.serializations</c> configuration property (a comma-delimited list of class names, each with
    ''' a public no-arg constructor). The Java default, Hadoop's <c>WritableSerialization</c>, does not
    ''' exist here, so with an unconfigured <see cref="Configuration"/> the factory holds no serializations
    ''' and logs a warning, as the Java original did. A class name that cannot be resolved (any exception
    ''' from <c>getClassByName</c>, not only Java's <c>ClassNotFoundException</c>) is skipped with a warning.
    ''' </summary>
    Public Class SerializationFactory
        Inherits Configured

        Private Shared ReadOnly LOG As Logger = LoggerFactory.getLogger(GetType(SerializationFactory))

        ' Serialization<?> in Java; the elements are Serialization(Of X) for various X
        Private ReadOnly serializations As IList(Of Object) = New List(Of Object)()

        ''' <summary>
        ''' <para>
        ''' Serializations are found by reading the <code>io.serializations</code>
        ''' property from <code>conf</code>, which is a comma-delimited list of
        ''' classnames.
        ''' </para>
        ''' </summary>
        Public Sub New(ByVal conf As Configuration)
            MyBase.New(conf)
            For Each serializerName As String In conf.getStrings("io.serializations", New String() {"org.apache.hadoop.io.serializer.WritableSerialization"})
                add(conf, serializerName)
            Next serializerName
        End Sub

        Private Sub add(ByVal conf As Configuration, ByVal serializationName As String)
            Try
                Dim serializationClass As Type = conf.getClassByName(serializationName)
                serializations.Add(ReflectionUtils.newInstance(Of Object)(serializationClass, Me.getConf()))
            Catch e As Exception
                LOG.warn("Serialization class not found: " & e.ToString())
            End Try
        End Sub

        Public Overridable Function getSerializer(Of T)(ByVal c As Type) As Serializer(Of T)
            Dim s As Serialization(Of T) = getSerialization(Of T)(c)
            Return If(s Is Nothing, Nothing, s.getSerializer(c))
        End Function

        Public Overridable Function getDeserializer(Of T)(ByVal c As Type) As Deserializer(Of T)
            Dim s As Serialization(Of T) = getSerialization(Of T)(c)
            Return If(s Is Nothing, Nothing, s.getDeserializer(c))
        End Function

        ''' <summary>
        ''' The first registered serialization that accepts the class, or Nothing. The Java code did an
        ''' unchecked cast; here a serialization whose type argument is not <typeparamref name="T"/> is skipped.
        ''' </summary>
        Public Overridable Function getSerialization(Of T)(ByVal c As Type) As Serialization(Of T)
            For Each serialization As Object In serializations
                If Accepts(serialization, c) Then
                    Dim typed As Serialization(Of T) = TryCast(serialization, Serialization(Of T))
                    If typed IsNot Nothing Then Return typed
                End If
            Next serialization
            Return Nothing
        End Function

        ''' <summary>Calls <c>accept(Type)</c> on a <c>Serialization(Of X)</c> whose X is unknown.</summary>
        Private Shared Function Accepts(ByVal serialization As Object, ByVal c As Type) As Boolean
            For Each itf As Type In serialization.GetType().GetInterfaces()
                If itf.IsGenericType AndAlso itf.GetGenericTypeDefinition() Is GetType(Serialization(Of )) Then
                    Dim m As Reflection.MethodInfo = itf.GetMethod("accept")
                    Return CBool(m.Invoke(serialization, New Object() {c}))
                End If
            Next itf
            Return False
        End Function
    End Class

End Namespace

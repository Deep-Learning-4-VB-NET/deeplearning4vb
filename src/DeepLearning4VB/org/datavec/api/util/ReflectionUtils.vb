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
Imports System.Threading
Imports Configurable = DeepLearning4VB.org.datavec.api.conf.Configurable
Imports Configuration = DeepLearning4VB.org.datavec.api.conf.Configuration
Imports DataInputBuffer = DeepLearning4VB.org.datavec.api.io.DataInputBuffer
Imports DataOutputBuffer = DeepLearning4VB.org.datavec.api.io.DataOutputBuffer
Imports DeepLearning4VB.org.datavec.api.io.serializers

Namespace org.datavec.api.util

    ''' <summary>
    ''' Hadoop-style reflection helpers: instantiate a class and hand it the configuration, and copy a
    ''' value through a <see cref="SerializationFactory"/>. The Java <c>setJobConf</c> hook (Hadoop
    ''' <c>JobConf</c>/<c>JobConfigurable</c> looked up by class name) has no counterpart here and is
    ''' dropped. <see cref="copy"/> throws <see cref="InvalidOperationException"/> when no serialization
    ''' accepts the class (Java failed with a <c>NullPointerException</c>).
    ''' </summary>
    <Obsolete("Hadoop compatibility helper; prefer direct construction")>
    Public Class ReflectionUtils

        Private Shared ReadOnly EMPTY_ARRAY() As Type = {}
        Private Shared serialFactory As SerializationFactory = Nothing

        Private Sub New()
        End Sub

        ''' <summary>
        ''' Create an object for the given class and initialize it from conf
        ''' </summary>
        ''' <param name="theClass"> class of which an object is created </param>
        ''' <param name="conf"> Configuration </param>
        ''' <returns> a new object </returns>
        Public Shared Function newInstance(Of T)(ByVal theClass As Type, ByVal conf As Configuration) As T
            Dim result As T = DeepLearning4VB.org.nd4j.common.io.ReflectionUtils.newInstance(Of T)(theClass)
            setConf(result, conf)
            Return result
        End Function

        ''' <summary>
        ''' Check and set 'configuration' if necessary.
        ''' </summary>
        ''' <param name="theObject"> object for which to set configuration </param>
        ''' <param name="conf"> Configuration </param>
        Public Shared Sub setConf(ByVal theObject As Object, ByVal conf As Configuration)
            If conf IsNot Nothing Then
                If TypeOf theObject Is Configurable Then
                    DirectCast(theObject, Configurable).setConf(conf)
                End If
            End If
        End Sub

        ''' <summary>
        ''' A pair of input/output buffers that we use to clone writables.
        ''' </summary>
        Private Class CopyInCopyOutBuffer
            Friend outBuffer As New DataOutputBuffer()
            Friend inBuffer As New DataInputBuffer()

            ''' <summary>
            ''' Move the data from the output buffer to the input buffer.
            ''' </summary>
            Friend Overridable Sub moveData()
                inBuffer.reset(outBuffer.Data, outBuffer.Length)
            End Sub
        End Class

        ''' <summary>
        ''' Allocate a buffer for each thread that tries to clone objects.
        ''' </summary>
        Private Shared ReadOnly cloneBuffers As New ThreadLocal(Of CopyInCopyOutBuffer)(Function() New CopyInCopyOutBuffer())

        Private Shared Function getFactory(ByVal conf As Configuration) As SerializationFactory
            If serialFactory Is Nothing Then
                serialFactory = New SerializationFactory(conf)
            End If
            Return serialFactory
        End Function

        ''' <summary>
        ''' Make a copy of the writable object using serialization to a buffer </summary>
        ''' <param name="src"> the object to copy from </param>
        ''' <param name="dst"> the object to copy into, which is destroyed </param>
        ''' <returns> the copy </returns>
        Public Shared Function copy(Of T)(ByVal conf As Configuration, ByVal src As T, ByVal dst As T) As T
            Dim buffer As CopyInCopyOutBuffer = cloneBuffers.Value
            buffer.outBuffer.reset()
            Dim factory As SerializationFactory = getFactory(conf)
            Dim cls As Type = src.GetType()
            Dim serializer As Serializer(Of T) = factory.getSerializer(Of T)(cls)
            If serializer Is Nothing Then
                Throw New InvalidOperationException("No serialization accepts " & cls.FullName)
            End If
            serializer.open(buffer.outBuffer.BaseStream)
            serializer.serialize(src)
            buffer.moveData()
            Dim deserializer As Deserializer(Of T) = factory.getDeserializer(Of T)(cls)
            deserializer.open(buffer.inBuffer.BaseStream)
            dst = deserializer.deserialize(dst)
            Return dst
        End Function
    End Class

End Namespace

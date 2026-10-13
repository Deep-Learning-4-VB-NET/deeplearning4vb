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
Imports System.Collections.Concurrent
Imports System.Reflection
Imports DeepLearning4VB.JavaCompat

Namespace org.datavec.api.writable

    ''' <summary>
    ''' Creates writables from their type key (the <see cref="WritableType"/> ordinal, or a custom key
    ''' registered with <see cref="registerWritableType"/>) and reads/writes them with the key prefixed.
    ''' The core types are registered on first use; <c>NDArray</c> (key 8) is skipped until
    ''' <c>NDArrayWritable</c> is ported.
    ''' </summary>
    Public Class WritableFactory

        Private Shared ReadOnly INSTANCE_FIELD As New WritableFactory()

        Private ReadOnly map As New ConcurrentDictionary(Of Short, Type)()
        Private ReadOnly constructorMap As New ConcurrentDictionary(Of Short, ConstructorInfo)()

        Private Sub New()
            For Each wt As WritableType In WritableTypeExtensions.values()
                If wt.isCoreWritable() AndAlso wt.getWritableClass() IsNot Nothing Then
                    registerWritableType(CShort(wt.ordinal()), wt.getWritableClass())
                End If
            Next wt
        End Sub

        ''' <returns> Singleton WritableFactory instance </returns>
        Public Shared ReadOnly Property Instance As WritableFactory
            Get
                Return INSTANCE_FIELD
            End Get
        End Property

        Public Shared Function getInstance() As WritableFactory
            Return INSTANCE_FIELD
        End Function

        ''' <summary>
        ''' Register a writable class with a specific key (as a short). Note that key values must be unique for each type of
        ''' Writable, as they are used as type information in certain types of serialisation. Consequently, an exception will
        ''' be thrown If the key value is not unique or is already assigned.<br/>
        ''' Note that in general, this method needs to only be used for custom Writable types; Care should be taken to ensure
        ''' that the given key does not change once assigned.
        ''' </summary>
        ''' <param name="writableTypeKey"> Key for the Writable </param>
        ''' <param name="writableClass">   Class for the given key. Must have a no-arg constructor </param>
        Public Overridable Sub registerWritableType(ByVal writableTypeKey As Short, ByVal writableClass As Type)
            If writableClass Is Nothing Then Throw New ArgumentNullException(NameOf(writableClass))
            If map.ContainsKey(writableTypeKey) Then
                Throw New NotSupportedException("Key " & writableTypeKey & " is already registered to type " & map(writableTypeKey).ToString() & " and cannot be registered to " & writableClass.ToString())
            End If

            Dim c As ConstructorInfo = writableClass.GetConstructor(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic, Nothing, Type.EmptyTypes, Nothing)
            If c Is Nothing Then
                Throw New InvalidOperationException("Cannot find no-arg constructor for class " & writableClass.ToString())
            End If

            map(writableTypeKey) = writableClass
            constructorMap(writableTypeKey) = c
        End Sub

        ''' <summary>
        ''' Create a new writable instance (using reflection) given the specified key
        ''' </summary>
        ''' <param name="writableTypeKey"> Key to create a new writable instance for </param>
        ''' <returns> A new (empty/default) Writable instance </returns>
        Public Overridable Function newWritable(ByVal writableTypeKey As Short) As Writable
            Dim c As ConstructorInfo = Nothing
            If Not constructorMap.TryGetValue(writableTypeKey, c) OrElse c Is Nothing Then
                Throw New InvalidOperationException("Unknown writable key: " & writableTypeKey)
            End If
            Try
                Return DirectCast(c.Invoke(Nothing), Writable)
            Catch e As Exception
                Throw New InvalidOperationException("Could not create new Writable instance", e)
            End Try
        End Function

        ''' <summary>
        ''' A convenience method for writing a given Writable  object to a DataOutput. The key is 1st written (a single short)
        ''' followed by the value from writable.
        ''' </summary>
        ''' <param name="w">          Writable value </param>
        ''' <param name="dataOutput"> DataOutput to write both key and value to </param>
        Public Overridable Sub writeWithType(ByVal w As Writable, ByVal dataOutput As DataOutput)
            w.writeType(dataOutput)
            w.write(dataOutput)
        End Sub

        ''' <summary>
        ''' Read a Writable From the DataInput, where the Writable was previously written using <seealso cref="writeWithType"/>
        ''' </summary>
        ''' <param name="dataInput"> DataInput to read the Writable from </param>
        ''' <returns> Writable from the DataInput </returns>
        Public Overridable Function readWithType(ByVal dataInput As DataInput) As Writable
            Dim w As Writable = newWritable(dataInput.readShort())
            w.readFields(dataInput)
            Return w
        End Function

    End Class

End Namespace

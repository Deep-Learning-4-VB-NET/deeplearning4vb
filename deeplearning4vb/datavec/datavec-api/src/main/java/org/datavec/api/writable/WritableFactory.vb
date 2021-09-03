Imports System
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull

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

Namespace org.datavec.api.writable


	Public Class WritableFactory

'JAVA TO VB CONVERTER NOTE: The field INSTANCE was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private Shared INSTANCE_Conflict As New WritableFactory()

		Private map As IDictionary(Of Short, Type) = New ConcurrentDictionary(Of Short, Type)()
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: private java.util.Map<Short, java.lang.reflect.Constructor<? extends Writable>> constructorMap = new java.util.concurrent.ConcurrentHashMap<>();
		Private constructorMap As IDictionary(Of Short, System.Reflection.ConstructorInfo(Of Writable)) = New ConcurrentDictionary(Of Short, System.Reflection.ConstructorInfo(Of Writable))()

		Private Sub New()
			For Each wt As WritableType In WritableType.values()
				If wt.isCoreWritable() Then
					registerWritableType(CShort(Math.Truncate(wt.ordinal())), wt.getWritableClass())
				End If
			Next wt
		End Sub

		''' <returns> Singleton WritableFactory instance </returns>
		Public Shared ReadOnly Property Instance As WritableFactory
			Get
				Return INSTANCE_Conflict
			End Get
		End Property

		''' <summary>
		''' Register a writable class with a specific key (as a short). Note that key values must be unique for each type of
		''' Writable, as they are used as type information in certain types of serialisation. Consequently, an exception will
		''' be thrown If the key value is not unique or is already assigned.<br>
		''' Note that in general, this method needs to only be used for custom Writable types; Care should be taken to ensure
		''' that the given key does not change once assigned.
		''' </summary>
		''' <param name="writableTypeKey"> Key for the Writable </param>
		''' <param name="writableClass">   Class for the given key. Must have a no-arg constructor </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void registerWritableType(short writableTypeKey, @NonNull @Class writableClass)
		Public Overridable Sub registerWritableType(ByVal writableTypeKey As Short, ByVal writableClass As Type)
			If map.ContainsKey(writableTypeKey) Then
				Throw New System.NotSupportedException("Key " & writableTypeKey & " is already registered to type " & map(writableTypeKey) & " and cannot be registered to " & writableClass)
			End If

'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.reflect.Constructor<? extends Writable> c;
			Dim c As System.Reflection.ConstructorInfo(Of Writable)
			Try
				c = writableClass.getDeclaredConstructor()
			Catch e As NoSuchMethodException
				Throw New Exception("Cannot find no-arg constructor for class " & writableClass)
			End Try

			map(writableTypeKey) = writableClass
			constructorMap(writableTypeKey) = c
		End Sub

		''' <summary>
		''' Create a new writable instance (using reflection) given the specified key
		''' </summary>
		''' <param name="writableTypeKey"> Key to create a new writable instance for </param>
		''' <returns> A new (empty/default) Writable instance </returns>
		Public Overridable Function newWritable(ByVal writableTypeKey As Short) As Writable
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.reflect.Constructor<? extends Writable> c = constructorMap.get(writableTypeKey);
			Dim c As System.Reflection.ConstructorInfo(Of Writable) = constructorMap(writableTypeKey)
			If c Is Nothing Then
				Throw New System.InvalidOperationException("Unknown writable key: " & writableTypeKey)
			End If
			Try
				Return c.Invoke()
			Catch e As Exception
				Throw New Exception("Could not create new Writable instance")
			End Try
		End Function

		''' <summary>
		''' A convenience method for writing a given Writable  object to a DataOutput. The key is 1st written (a single short)
		''' followed by the value from writable.
		''' </summary>
		''' <param name="w">          Writable value </param>
		''' <param name="dataOutput"> DataOutput to write both key and value to </param>
		''' <exception cref="IOException"> If an error occurs during writing to the DataOutput </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public void writeWithType(Writable w, java.io.DataOutput dataOutput) throws java.io.IOException
		Public Overridable Sub writeWithType(ByVal w As Writable, ByVal dataOutput As DataOutput)
			w.writeType(dataOutput)
			w.write(dataOutput)
		End Sub

		''' <summary>
		''' Read a Writable From the DataInput, where the Writable was previously written using <seealso cref="writeWithType(Writable, DataOutput)"/>
		''' </summary>
		''' <param name="dataInput"> DataInput to read the Writable from </param>
		''' <returns> Writable from the DataInput </returns>
		''' <exception cref="IOException"> In an error occurs during reading </exception>
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: public Writable readWithType(java.io.DataInput dataInput) throws java.io.IOException
		Public Overridable Function readWithType(ByVal dataInput As DataInput) As Writable
			Dim w As Writable = newWritable(dataInput.readShort())
			w.readFields(dataInput)
			Return w
		End Function

	End Class

End Namespace
Imports System
Imports Random = org.nd4j.linalg.api.rng.Random

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

Namespace org.nd4j.linalg.factory


	Public Class RandomFactory
		Private threadRandom As New ThreadLocal(Of Random)()
		Private randomClass As Type

		Public Sub New(ByVal randomClass As Type)
			Me.randomClass = randomClass
		End Sub

		''' <summary>
		''' This method returns Random implementation instance associated with calling thread
		''' </summary>
		''' <returns> object implementing Random interface </returns>
		Public Overridable ReadOnly Property Random As Random
			Get
				Try
					If threadRandom.get() Is Nothing Then
						Dim t As Random = DirectCast(System.Activator.CreateInstance(randomClass), Random)
						If t.StatePointer IsNot Nothing Then
							' TODO: attach this thing to deallocator
							' if it's stateless random - we just don't care then
						End If
						threadRandom.set(t)
						Return t
					End If
    
    
					Return threadRandom.get()
				Catch e As Exception
					Throw New Exception(e)
				End Try
			End Get
		End Property

		''' <summary>
		''' This method returns new onject implementing Random interface, initialized with System.currentTimeMillis() as seed
		''' </summary>
		''' <returns> object implementing Random interface </returns>
		Public Overridable ReadOnly Property NewRandomInstance As Random
			Get
				Return getNewRandomInstance(DateTimeHelper.CurrentUnixTimeMillis())
			End Get
		End Property


		''' <summary>
		''' This method returns new onject implementing Random interface, initialized with seed value
		''' </summary>
		''' <param name="seed"> seed for this rng object </param>
		''' <returns> object implementing Random interface </returns>
		Public Overridable Function getNewRandomInstance(ByVal seed As Long) As Random
			Try
				Dim t As Random = DirectCast(System.Activator.CreateInstance(randomClass), Random)
				If t.StatePointer IsNot Nothing Then
					' TODO: attach this thing to deallocator
					' if it's stateless random - we just don't care then
				End If
				t.Seed = seed
				Return t
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function

		''' <summary>
		''' This method returns a new object implementing <seealso cref="System.Random"/>
		''' interface, initialized with seed value, with size of elements in buffer
		''' </summary>
		''' <param name="seed"> rng seed </param>
		''' <param name="size"> size of underlying buffer </param>
		''' <returns> object implementing Random interface </returns>
		Public Overridable Function getNewRandomInstance(ByVal seed As Long, ByVal size As Long) As Random
			Try
				Dim c As Type = randomClass
'JAVA TO VB CONVERTER WARNING: Java wildcard generics have no direct equivalent in VB:
'ORIGINAL LINE: java.lang.reflect.Constructor<?> constructor = c.getConstructor(long.class, long.class);
				Dim constructor As System.Reflection.ConstructorInfo(Of Object) = c.GetConstructor(GetType(Long), GetType(Long))
				Dim t As Random = DirectCast(constructor.Invoke(seed, size), Random)
				Return t
			Catch e As Exception
				Throw New Exception(e)
			End Try
		End Function
	End Class

End Namespace
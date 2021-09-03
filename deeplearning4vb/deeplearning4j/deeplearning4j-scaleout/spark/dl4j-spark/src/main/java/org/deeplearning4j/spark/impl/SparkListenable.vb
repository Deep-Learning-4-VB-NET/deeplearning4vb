Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports NonNull = lombok.NonNull
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports StatsStorageRouter = org.deeplearning4j.core.storage.StatsStorageRouter
Imports StatsStorageRouterProvider = org.deeplearning4j.core.storage.StatsStorageRouterProvider
Imports RoutingIterationListener = org.deeplearning4j.core.storage.listener.RoutingIterationListener
Imports TrainingListener = org.deeplearning4j.optimize.api.TrainingListener
Imports org.deeplearning4j.spark.api
Imports VanillaStatsStorageRouterProvider = org.deeplearning4j.spark.impl.listeners.VanillaStatsStorageRouterProvider

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

Namespace org.deeplearning4j.spark.impl


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class SparkListenable
	Public Class SparkListenable

		Protected Friend trainingMaster As TrainingMaster
'JAVA TO VB CONVERTER NOTE: The field listeners was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private listeners_Conflict As IList(Of TrainingListener) = New List(Of TrainingListener)()


		''' <summary>
		''' This method allows you to specify trainingListeners for this model.
		''' </summary>
		''' <param name="listeners"> Iteration listeners </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setListeners(@NonNull Collection<org.deeplearning4j.optimize.api.TrainingListener> listeners)
		Public Overridable WriteOnly Property Listeners As ICollection(Of TrainingListener)
			Set(ByVal listeners As ICollection(Of TrainingListener))
				Me.listeners_Conflict.Clear()
				CType(Me.listeners_Conflict, List(Of TrainingListener)).AddRange(listeners)
				If trainingMaster IsNot Nothing Then
					trainingMaster.setListeners(Me.listeners_Conflict)
				End If
			End Set
		End Property

		''' <summary>
		''' This method allows you to specify trainingListeners for this model. Note that for listeners
		''' like StatsListener (that have state that will be sent somewhere), consider instead using {@link
		''' #setListeners(StatsStorageRouter, Collection)}
		''' </summary>
		''' <param name="listeners"> Listeners to set </param>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public void setListeners(@NonNull TrainingListener... listeners)
		Public Overridable WriteOnly Property Listeners As TrainingListener()
			Set(ByVal listeners() As TrainingListener)
				setListeners(Arrays.asList(listeners))
			End Set
		End Property

		''' <summary>
		''' Set the listeners, along with a StatsStorageRouter that the results will be shuffled to (in the
		''' case of any listeners that implement the <seealso cref="RoutingIterationListener"/> interface)
		''' </summary>
		''' <param name="statsStorage"> Stats storage router to place the results into </param>
		''' <param name="listeners"> Listeners to set </param>
		Public Overridable Sub setListeners(ByVal statsStorage As StatsStorageRouter, ParamArray ByVal listeners() As TrainingListener)
			setListeners(statsStorage, Arrays.asList(listeners))
		End Sub

		''' <summary>
		''' Set the listeners, along with a StatsStorageRouter that the results will be shuffled to (in the
		''' case of any listeners that implement the <seealso cref="RoutingIterationListener"/> interface)
		''' </summary>
		''' <param name="statsStorage"> Stats storage router to place the results into </param>
		''' <param name="listeners"> Listeners to set </param>
		Public Overridable Sub setListeners(Of T1 As TrainingListener)(ByVal statsStorage As StatsStorageRouter, ByVal listeners As ICollection(Of T1))
			'Check if we have any RoutingIterationListener instances that need a StatsStorage implementation...
			Dim routerProvider As StatsStorageRouterProvider = Nothing
			If listeners IsNot Nothing Then
				For Each l As TrainingListener In listeners
					If TypeOf l Is RoutingIterationListener Then
						Dim rl As RoutingIterationListener = DirectCast(l, RoutingIterationListener)
						If statsStorage Is Nothing AndAlso rl.StorageRouter Is Nothing Then
							log.warn("RoutingIterationListener provided without providing any StatsStorage instance. Iterator may not function without one. Listener: {}", l)
						ElseIf rl.StorageRouter IsNot Nothing AndAlso Not (TypeOf rl.StorageRouter Is Serializable) Then
							'Spark would throw a (probably cryptic) serialization exception later anyway...
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
							Throw New System.InvalidOperationException("RoutingIterationListener provided with non-serializable storage router " & vbLf & "RoutingIterationListener class: " & rl.GetType().FullName & vbLf & "StatsStorageRouter class: " & rl.StorageRouter.GetType().FullName)
						End If

						'Need to give workers a router provider...
						If routerProvider Is Nothing Then
							routerProvider = New VanillaStatsStorageRouterProvider()
						End If
					End If
				Next l
			End If
			Me.listeners_Conflict.Clear()
			If listeners IsNot Nothing Then
				CType(Me.listeners_Conflict, List(Of TrainingListener)).AddRange(listeners)
				If trainingMaster IsNot Nothing Then
					trainingMaster.setListeners(statsStorage, Me.listeners_Conflict)
				End If
			End If
		End Sub
	End Class

End Namespace
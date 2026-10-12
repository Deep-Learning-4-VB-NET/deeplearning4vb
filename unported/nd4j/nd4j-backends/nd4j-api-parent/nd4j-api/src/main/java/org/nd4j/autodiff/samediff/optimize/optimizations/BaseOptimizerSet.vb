Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports OptimizerSet = org.nd4j.autodiff.samediff.optimize.OptimizerSet

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

Namespace org.nd4j.autodiff.samediff.optimize.optimizations


	''' 
	''' <summary>
	''' @author Alex Black
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public abstract class BaseOptimizerSet implements org.nd4j.autodiff.samediff.optimize.OptimizerSet
	Public MustInherit Class BaseOptimizerSet
		Implements OptimizerSet

		Public Overridable ReadOnly Property Optimizers As IList(Of Optimizer) Implements OptimizerSet.getOptimizers
			Get
				Dim methods() As System.Reflection.MethodInfo = Me.GetType().GetMethods(BindingFlags.DeclaredOnly Or BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Static Or BindingFlags.Instance)
				Dim [out] As IList(Of Optimizer) = New List(Of Optimizer)(methods.Length)
				For Each m As System.Reflection.MethodInfo In methods
					Dim modifiers As Integer = m.getModifiers()
					Dim retType As Type = m.getReturnType()
					If retType IsNot Nothing AndAlso Modifier.isPublic(modifiers) AndAlso retType.IsAssignableFrom(GetType(Optimizer)) Then
						Try
							Dim o As Optimizer = DirectCast(m.invoke(Nothing), Optimizer)
							[out].Add(o)
						Catch e As Exception When TypeOf e Is IllegalAccessException OrElse TypeOf e Is InvocationTargetException
							log.warn("Could not create optimizer from method: {}", m, e)
						End Try
					End If
				Next m
    
				Dim declaredClasses() As Type = Me.GetType().GetNestedTypes(BindingFlags.Public Or BindingFlags.NonPublic)
				For Each c As Type In declaredClasses
					Dim modifiers As Integer = c.getModifiers()
					If Modifier.isPublic(modifiers) AndAlso Not Modifier.isAbstract(modifiers) AndAlso c.IsAssignableFrom(GetType(Optimizer)) Then
						Try
							[out].Add(DirectCast(System.Activator.CreateInstance(c), Optimizer))
						Catch e As Exception When TypeOf e Is IllegalAccessException OrElse TypeOf e Is InstantiationException
							log.warn("Could not create optimizer from inner class: {}", c, e)
						End Try
					End If
				Next c
    
				Return [out]
			End Get
		End Property
	End Class

End Namespace
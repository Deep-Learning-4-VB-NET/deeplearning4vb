Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DL4JInvalidConfigException = org.deeplearning4j.exception.DL4JInvalidConfigException
Imports LayerConstraint = org.deeplearning4j.nn.api.layers.LayerConstraint
Imports IDropout = org.deeplearning4j.nn.conf.dropout.IDropout
Imports FrozenLayer = org.deeplearning4j.nn.conf.layers.misc.FrozenLayer
Imports Bidirectional = org.deeplearning4j.nn.conf.layers.recurrent.Bidirectional
Imports L1Regularization = org.nd4j.linalg.learning.regularization.L1Regularization
Imports L2Regularization = org.nd4j.linalg.learning.regularization.L2Regularization
Imports Regularization = org.nd4j.linalg.learning.regularization.Regularization

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

Namespace org.deeplearning4j.nn.conf.layers


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class LayerValidation
	Public Class LayerValidation

		Private Sub New()
		End Sub

		''' <summary>
		''' Asserts that the layer nIn and nOut values are set for the layer
		''' </summary>
		''' <param name="layerType">     Type of layer ("DenseLayer", etc) </param>
		''' <param name="layerName">     Name of the layer (may be null if not set) </param>
		''' <param name="layerIndex">    Index of the layer </param>
		''' <param name="nIn">           nIn value </param>
		''' <param name="nOut">          nOut value </param>
		Public Shared Sub assertNInNOutSet(ByVal layerType As String, ByVal layerName As String, ByVal layerIndex As Long, ByVal nIn As Long, ByVal nOut As Long)
			If nIn <= 0 OrElse nOut <= 0 Then
				If layerName Is Nothing Then
					layerName = "(name not set)"
				End If
				Throw New DL4JInvalidConfigException(layerType & " (index=" & layerIndex & ", name=" & layerName & ") nIn=" & nIn & ", nOut=" & nOut & "; nIn and nOut must be > 0")
			End If
		End Sub

		''' <summary>
		''' Asserts that the layer nOut value is set for the layer
		''' </summary>
		''' <param name="layerType">     Type of layer ("DenseLayer", etc) </param>
		''' <param name="layerName">     Name of the layer (may be null if not set) </param>
		''' <param name="layerIndex">    Index of the layer </param>
		''' <param name="nOut">          nOut value </param>
		Public Shared Sub assertNOutSet(ByVal layerType As String, ByVal layerName As String, ByVal layerIndex As Long, ByVal nOut As Long)
			If nOut <= 0 Then
				If layerName Is Nothing Then
					layerName = "(name not set)"
				End If
				Throw New DL4JInvalidConfigException(layerType & " (index=" & layerIndex & ", name=" & layerName & ") nOut=" & nOut & "; nOut must be > 0")
			End If
		End Sub

		Public Shared Sub generalValidation(ByVal layerName As String, ByVal layer As Layer, ByVal iDropout As IDropout, ByVal regularization As IList(Of Regularization), ByVal regularizationBias As IList(Of Regularization), ByVal allParamConstraints As IList(Of LayerConstraint), ByVal weightConstraints As IList(Of LayerConstraint), ByVal biasConstraints As IList(Of LayerConstraint))

			If layer IsNot Nothing Then
				If TypeOf layer Is BaseLayer Then
					Dim bLayer As BaseLayer = DirectCast(layer, BaseLayer)
					configureBaseLayer(layerName, bLayer, iDropout, regularization, regularizationBias)
				ElseIf TypeOf layer Is FrozenLayer AndAlso TypeOf (DirectCast(layer, FrozenLayer)).getLayer() Is BaseLayer Then
					Dim bLayer As BaseLayer = CType(DirectCast(layer, FrozenLayer).getLayer(), BaseLayer)
					configureBaseLayer(layerName, bLayer, iDropout, regularization, regularizationBias)
				ElseIf TypeOf layer Is Bidirectional Then
					Dim l As Bidirectional = DirectCast(layer, Bidirectional)
					generalValidation(layerName, l.getFwd(), iDropout, regularization, regularizationBias, allParamConstraints, weightConstraints, biasConstraints)
					generalValidation(layerName, l.getBwd(), iDropout, regularization, regularizationBias, allParamConstraints, weightConstraints, biasConstraints)
				End If

				If layer.getConstraints() Is Nothing OrElse layer.constraints.Count = 0 Then
					Dim allConstraints As IList(Of LayerConstraint) = New List(Of LayerConstraint)()
					If allParamConstraints IsNot Nothing AndAlso layer.initializer().paramKeys(layer).Count > 0 Then
						For Each c As LayerConstraint In allConstraints
							Dim c2 As LayerConstraint = c.clone()
							c2.Params = New HashSet(Of String)(layer.initializer().paramKeys(layer))
							allConstraints.Add(c2)
						Next c
					End If

					If weightConstraints IsNot Nothing AndAlso layer.initializer().weightKeys(layer).Count > 0 Then
						For Each c As LayerConstraint In weightConstraints
							Dim c2 As LayerConstraint = c.clone()
							c2.Params = New HashSet(Of String)(layer.initializer().weightKeys(layer))
							allConstraints.Add(c2)
						Next c
					End If

					If biasConstraints IsNot Nothing AndAlso layer.initializer().biasKeys(layer).Count > 0 Then
						For Each c As LayerConstraint In biasConstraints
							Dim c2 As LayerConstraint = c.clone()
							c2.Params = New HashSet(Of String)(layer.initializer().biasKeys(layer))
							allConstraints.Add(c2)
						Next c
					End If

					If allConstraints.Count > 0 Then
						layer.setConstraints(allConstraints)
					Else
						layer.setConstraints(Nothing)
					End If
				End If
			End If
		End Sub

		Private Shared Sub configureBaseLayer(ByVal layerName As String, ByVal bLayer As BaseLayer, ByVal iDropout As IDropout, ByVal regularization As IList(Of Regularization), ByVal regularizationBias As IList(Of Regularization))
			If regularization IsNot Nothing AndAlso regularization.Count > 0 Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.learning.regularization.Regularization> bLayerRegs = new java.util.ArrayList<>(bLayer.getRegularization());
				Dim bLayerRegs As IList(Of Regularization) = New List(Of Regularization)(bLayer.getRegularization())
				If bLayerRegs Is Nothing OrElse bLayerRegs.Count = 0 Then
					bLayer.setRegularization(regularization)
				Else
					Dim hasL1 As Boolean = False
					Dim hasL2 As Boolean = False
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.learning.regularization.Regularization> regContext = regularization;
					Dim regContext As IList(Of Regularization) = regularization
					For Each reg As Regularization In bLayerRegs
						If TypeOf reg Is L1Regularization Then
							hasL1 = True
						ElseIf TypeOf reg Is L2Regularization Then
							hasL2 = True
						End If
					Next reg
					For Each reg As Regularization In regContext
						If TypeOf reg Is L1Regularization Then
							If Not hasL1 Then
								bLayerRegs.Add(reg)
							End If
						ElseIf TypeOf reg Is L2Regularization Then
							If Not hasL2 Then
								bLayerRegs.Add(reg)
							End If
						Else
							bLayerRegs.Add(reg)
						End If
					Next reg
				End If

				bLayer.setRegularization(bLayerRegs)
			End If


			If regularizationBias IsNot Nothing AndAlso regularizationBias.Count > 0 Then
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.learning.regularization.Regularization> bLayerRegs = bLayer.getRegularizationBias();
				Dim bLayerRegs As IList(Of Regularization) = bLayer.getRegularizationBias()
				If bLayerRegs Is Nothing OrElse bLayerRegs.Count = 0 Then
					bLayer.setRegularizationBias(regularizationBias)
				Else
					Dim hasL1 As Boolean = False
					Dim hasL2 As Boolean = False
'JAVA TO VB CONVERTER WARNING: The original Java variable was marked 'final':
'ORIGINAL LINE: final java.util.List<org.nd4j.linalg.learning.regularization.Regularization> regContext = regularizationBias;
					Dim regContext As IList(Of Regularization) = regularizationBias
					For Each reg As Regularization In bLayerRegs
						If TypeOf reg Is L1Regularization Then
							hasL1 = True
						ElseIf TypeOf reg Is L2Regularization Then
							hasL2 = True
						End If
					Next reg
					For Each reg As Regularization In regContext
						If TypeOf reg Is L1Regularization Then
							If Not hasL1 Then
								bLayerRegs.Add(reg)
							End If
						ElseIf TypeOf reg Is L2Regularization Then

							If Not hasL2 Then
								bLayerRegs.Add(reg)
							End If
						Else
							bLayerRegs.Add(reg)
						End If
					Next reg
				End If

				bLayer.setRegularizationBias(bLayerRegs)
			End If

			If bLayer.getIDropout() Is Nothing Then
				bLayer.setIDropout(iDropout)
			End If
		End Sub
	End Class

End Namespace
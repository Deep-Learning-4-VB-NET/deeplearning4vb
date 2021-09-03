Imports System.Collections.Generic
Imports val = lombok.val
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports AmsGradUpdater = org.nd4j.linalg.api.ops.impl.updaters.AmsGradUpdater
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports AMSGrad = org.nd4j.linalg.learning.config.AMSGrad
Imports AdaDelta = org.nd4j.linalg.learning.config.AdaDelta
Imports AdaGrad = org.nd4j.linalg.learning.config.AdaGrad
Imports AdaMax = org.nd4j.linalg.learning.config.AdaMax
Imports Adam = org.nd4j.linalg.learning.config.Adam
Imports Nadam = org.nd4j.linalg.learning.config.Nadam
Imports Nesterovs = org.nd4j.linalg.learning.config.Nesterovs
Imports RmsProp = org.nd4j.linalg.learning.config.RmsProp
Imports Sgd = org.nd4j.linalg.learning.config.Sgd
import static org.junit.jupiter.api.Assertions.assertEquals

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
Namespace org.nd4j.linalg.learning


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.TRAINING) @NativeTag @Tag(TagNames.DL4J_OLD_API) public class UpdaterValidation extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class UpdaterValidation
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaDeltaUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaDeltaUpdater(ByVal backend As Nd4jBackend)
			Dim rho As Double = 0.95
			Dim epsilon As Double = 1e-6

			Dim msg As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim msdx As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("msg") = msg.dup()
			state("msdx") = msdx.dup()
			Dim u As AdaDeltaUpdater = CType((New AdaDelta(rho,epsilon)).instantiate(state, True), AdaDeltaUpdater)

			assertEquals(msg, state("msg"))
			assertEquals(msdx, state("msdx"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim msgu As val = msg.dup()
				Dim msdxu As val = msdx.dup()

				UpdaterJavaCode.applyAdaDeltaUpdater(g1, msg, msdx, rho, epsilon)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaDeltaUpdater(g3, msgu, msdxu, rho, epsilon))

				assertEquals(msg, state("msg"))
				assertEquals(msdx, state("msdx"))
				assertEquals(g1, g2)

				assertEquals(msg, msgu)
				assertEquals(msdx, msdxu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaGradUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaGradUpdater(ByVal backend As Nd4jBackend)
			Dim lr As Double = 0.1
			Dim epsilon As Double = 1e-6

			Dim s As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("grad") = s.dup()
			Dim u As AdaGradUpdater = CType((New AdaGrad(lr, epsilon)).instantiate(state, True), AdaGradUpdater)

			assertEquals(s, state("grad"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim su As val = s.dup()

				UpdaterJavaCode.applyAdaGradUpdater(g1, s, lr, epsilon)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaGradUpdater(g3, su, lr, epsilon))

				assertEquals(s, state("grad"))
				assertEquals(g1, g2)

				assertEquals(s, su)
				assertEquals(g1, g3)
			Next i
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdamUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdamUpdater(ByVal backend As Nd4jBackend)

			Dim lr As Double = 1e-3
			Dim beta1 As Double = 0.9
			Dim beta2 As Double = 0.999
			Dim eps As Double = 1e-8

			Dim m As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim v As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("M") = m.dup()
			state("V") = v.dup()
			Dim u As AdamUpdater = CType((New Adam(lr, beta1, beta2, eps)).instantiate(state, True), AdamUpdater)

			assertEquals(m, state("M"))
			assertEquals(v, state("V"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim mu As val = m.dup()
				Dim vu As val = v.dup()

				UpdaterJavaCode.applyAdamUpdater(g1, m, v, lr, beta1, beta2, eps, i)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdamUpdater(g3, vu, mu, lr, beta1, beta2, eps, i))

				assertEquals(m, state("M"))
				assertEquals(v, state("V"))
				assertEquals(g1, g2)

				assertEquals(m, mu)
				assertEquals(v, vu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAdaMaxUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAdaMaxUpdater(ByVal backend As Nd4jBackend)
			Dim lr As Double = 1e-3
			Dim beta1 As Double = 0.9
			Dim beta2 As Double = 0.999
			Dim eps As Double = 1e-8

			Dim m As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim v As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("M") = m.dup()
			state("V") = v.dup()
			Dim u As AdaMaxUpdater = CType((New AdaMax(lr, beta1, beta2, eps)).instantiate(state, True), AdaMaxUpdater)

			assertEquals(m, state("M"))
			assertEquals(v, state("V"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim mu As val = m.dup()
				Dim vu As val = v.dup()

				UpdaterJavaCode.applyAdaMaxUpdater(g1, m, v, lr, beta1, beta2, eps, i)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.AdaMaxUpdater(g3, vu, mu, lr, beta1, beta2, eps, i))

				assertEquals(m, state("M"))
				assertEquals(v, state("V"))
				assertEquals(g1, g2)

				assertEquals(m, mu)
				assertEquals(v, vu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testAmsGradUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testAmsGradUpdater(ByVal backend As Nd4jBackend)
			Dim lr As Double = 1e-3
			Dim beta1 As Double = 0.9
			Dim beta2 As Double = 0.999
			Dim eps As Double = 1e-8

			Dim m As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim v As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim vH As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("M") = m.dup()
			state("V") = v.dup()
			state("V_HAT") = vH.dup()
			Dim u As AMSGradUpdater = CType((New AMSGrad(lr, beta1, beta2, eps)).instantiate(state, True), AMSGradUpdater)

			assertEquals(m, state("M"))
			assertEquals(v, state("V"))
			assertEquals(vH, state("V_HAT"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim mu As val = m.dup()
				Dim vu As val = v.dup()
				Dim hu As val = vH.dup()

				UpdaterJavaCode.applyAmsGradUpdater(g1, m, v, vH, lr, beta1, beta2, eps, i)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New AmsGradUpdater(g3, vu, mu, hu, lr, beta1, beta2, eps, i))

				assertEquals(m, state("M"))
				assertEquals(v, state("V"))
				assertEquals(vH, state("V_HAT"))
				assertEquals(g1, g2)

				assertEquals(m, mu)
				assertEquals(v, vu)
				assertEquals(vH, hu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNadamUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNadamUpdater(ByVal backend As Nd4jBackend)

			Dim lr As Double = 1e-3
			Dim beta1 As Double = 0.9
			Dim beta2 As Double = 0.999
			Dim eps As Double = 1e-8

			Dim m As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)
			Dim v As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("M") = m.dup()
			state("V") = v.dup()
			Dim u As NadamUpdater = CType((New Nadam(lr, beta1, beta2, eps)).instantiate(state, True), NadamUpdater)

			assertEquals(m, state("M"))
			assertEquals(v, state("V"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim vu As val = v.dup()
				Dim mu As val = m.dup()

				UpdaterJavaCode.applyNadamUpdater(g1, m, v, lr, beta1, beta2, eps, i)

				u.applyUpdater(g2, i, 0)

				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.NadamUpdater(g3, vu, mu, lr, beta1, beta2, eps, i))

				assertEquals(m, state("M"))
				assertEquals(v, state("V"))
				assertEquals(g1, g2)

				assertEquals(m, mu)
				assertEquals(v, vu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNesterovUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testNesterovUpdater(ByVal backend As Nd4jBackend)

			Dim lr As Double = 0.1
			Dim momentum As Double = 0.9

			Dim v As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("V") = v.dup()
			Dim u As NesterovsUpdater = CType((New Nesterovs(lr, momentum)).instantiate(state, True), NesterovsUpdater)

			assertEquals(v, state("V"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim vu As val = v.dup()

				UpdaterJavaCode.applyNesterovsUpdater(g1, v, lr, momentum)
				u.applyUpdater(g2, i, 0)
				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.NesterovsUpdater(g3, vu, lr, momentum))

				assertEquals(v, state("V"))
				assertEquals(g1, g2)

				assertEquals(v, vu)
				assertEquals(g1, g3)
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRmsPropUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRmsPropUpdater(ByVal backend As Nd4jBackend)

			Dim lr As Double = 0.1
			Dim decay As Double = 0.95
			Dim eps As Double = 1e-8

			Dim g As INDArray = Nd4j.zeros(DataType.DOUBLE, 1, 5)

			Dim state As IDictionary(Of String, INDArray) = New Dictionary(Of String, INDArray)()
			state("G") = g.dup()
			Dim u As RmsPropUpdater = CType((New RmsProp(lr, decay, eps)).instantiate(state, True), RmsPropUpdater)

			assertEquals(g, state("G"))

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()
				Dim gu As val = g.dup()

				UpdaterJavaCode.applyRmsProp(g1, g, lr, decay, eps)
				u.applyUpdater(g2, i, 0)
				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.RmsPropUpdater(g3, gu, lr,decay, eps))

				assertEquals(g, state("G"))
				assertEquals(g1, g2)

				assertEquals(g, gu)
				assertEquals(g1, g3)

			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testSgdUpdater(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testSgdUpdater(ByVal backend As Nd4jBackend)
			Dim lr As Double = 0.1

			Dim u As SgdUpdater = CType((New Sgd(lr)).instantiate(DirectCast(Nothing, IDictionary(Of String, INDArray)), True), SgdUpdater)

			For i As Integer = 0 To 2
				Dim g1 As INDArray = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(ChrW(1), 5)
				Dim g2 As INDArray = g1.dup()
				Dim g3 As val = g1.dup()

				UpdaterJavaCode.applySgd(g1, lr)
				Nd4j.exec(New org.nd4j.linalg.api.ops.impl.updaters.SgdUpdater(g3, lr))

				u.applyUpdater(g2, i, 0)
				assertEquals(g1, g2)
				assertEquals(g1, g3)
			Next i
		End Sub


	'    
	'      @ParameterizedTest
	'    @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs")
	'    public void createUpdaterTestCases(Nd4jBackend backend) {
	'        Nd4j.create(1);
	'        Nd4j.getRandom().setSeed(12345);
	'
	'        int size = 5;
	'
	'        for(boolean random : new boolean[]{false, true}) {
	'            System.out.println("/////////////////////////////// " + (random ? "RANDOM TEST CASES" : "LINSPACE TEST CASES") + " ///////////////////////////////" );
	'
	'            for (IUpdater u : new IUpdater[]{new AdaDelta(), new Adam(), new AdaMax(), new AMSGrad(), new Nadam(), new Nesterovs(), new RmsProp(), new Sgd()}) {
	'
	'                System.out.println(" ===== " + u + " =====");
	'
	'                long ss = u.stateSize(size);
	'                INDArray state = ss > 0 ? Nd4j.create(DataType.DOUBLE, 1, ss) : null;
	'                GradientUpdater gu = u.instantiate(state, true);
	'
	'                System.out.println("Initial state:");
	'                Map<String, INDArray> m = gu.getState();
	'                for (String s : m.keySet()) {
	'                    System.out.println("state: " + s + " - " + m.get(s).toStringFull());
	'                }
	'
	'                for (int i = 0; i < 3; i++) {
	'                    System.out.println("Iteration: " + i);
	'                    INDArray in;
	'                    if(random){
	'                        in = Nd4j.rand(DataType.DOUBLE, 1, 5);
	'                    } else {
	'                        in = Nd4j.linspace(DataType.DOUBLE, 1, 5, 1).reshape(1, 5);
	'                    }
	'
	'                    System.out.println("grad: " + in.toStringFull());
	'                    gu.applyUpdater(in, 0, 0);
	'                    System.out.println("update: " + in.toStringFull());
	'
	'                    m = gu.getState();
	'                    for (String s : m.keySet()) {
	'                        System.out.println("state: " + s + " - " + m.get(s).toStringFull());
	'                    }
	'                }
	'            }
	'        }
	'    }
	'    
	End Class

End Namespace
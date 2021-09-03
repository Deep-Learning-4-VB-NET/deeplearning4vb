Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Microsoft.VisualBasic
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue
Imports FileUtils = org.apache.commons.io.FileUtils
Imports FilenameUtils = org.apache.commons.io.FilenameUtils
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException
Imports ImportClassMapping = org.nd4j.imports.converters.ImportClassMapping
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DynamicCustomOp = org.nd4j.linalg.api.ops.DynamicCustomOp
Imports NoOp = org.nd4j.linalg.api.ops.NoOp
Imports CompatSparseToDense = org.nd4j.linalg.api.ops.compat.CompatSparseToDense
Imports CompatStringSplit = org.nd4j.linalg.api.ops.compat.CompatStringSplit
Imports BarnesEdgeForces = org.nd4j.linalg.api.ops.custom.BarnesEdgeForces
Imports BarnesHutGains = org.nd4j.linalg.api.ops.custom.BarnesHutGains
Imports BarnesHutSymmetrize = org.nd4j.linalg.api.ops.custom.BarnesHutSymmetrize
Imports KnnMinDistance = org.nd4j.linalg.api.ops.custom.KnnMinDistance
Imports SpTreeCell = org.nd4j.linalg.api.ops.custom.SpTreeCell
Imports BroadcastAMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMax
Imports BroadcastAMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAMin
Imports BroadcastAddOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastAddOp
Imports BroadcastCopyOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastCopyOp
Imports BroadcastDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastDivOp
Imports BroadcastGradientArgs = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastGradientArgs
Imports BroadcastMax = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMax
Imports BroadcastMin = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMin
Imports BroadcastMulOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastMulOp
Imports BroadcastRDivOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastRDivOp
Imports BroadcastRSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastRSubOp
Imports BroadcastSubOp = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastSubOp
Imports BroadcastTo = org.nd4j.linalg.api.ops.impl.broadcast.BroadcastTo
Imports BroadcastEqualTo = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastEqualTo
Imports BroadcastGreaterThan = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastGreaterThan
Imports BroadcastGreaterThanOrEqual = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastGreaterThanOrEqual
Imports BroadcastLessThan = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastLessThan
Imports BroadcastLessThanOrEqual = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastLessThanOrEqual
Imports BroadcastNotEqual = org.nd4j.linalg.api.ops.impl.broadcast.bool.BroadcastNotEqual
Imports Enter = org.nd4j.linalg.api.ops.impl.controlflow.compat.Enter
Imports [Exit] = org.nd4j.linalg.api.ops.impl.controlflow.compat.Exit
Imports LoopCond = org.nd4j.linalg.api.ops.impl.controlflow.compat.LoopCond
Imports Merge = org.nd4j.linalg.api.ops.impl.controlflow.compat.Merge
Imports NextIteration = org.nd4j.linalg.api.ops.impl.controlflow.compat.NextIteration
Imports Switch = org.nd4j.linalg.api.ops.impl.controlflow.compat.Switch
Imports FreeGridOp = org.nd4j.linalg.api.ops.impl.grid.FreeGridOp
Imports ArgMax = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMax
Imports ArgMin = org.nd4j.linalg.api.ops.impl.indexaccum.custom.ArgMin
Imports ExternalErrorsFunction = org.nd4j.linalg.api.ops.impl.layers.ExternalErrorsFunction
Imports DeConv2DTF = org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv2DTF
Imports DeConv3DTF = org.nd4j.linalg.api.ops.impl.layers.convolution.DeConv3DTF
Imports InvertedPredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.InvertedPredicateMetaOp
Imports PostulateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PostulateMetaOp
Imports PredicateMetaOp = org.nd4j.linalg.api.ops.impl.meta.PredicateMetaOp
Imports ReduceMetaOp = org.nd4j.linalg.api.ops.impl.meta.ReduceMetaOp
Imports CbowRound = org.nd4j.linalg.api.ops.impl.nlp.CbowRound
Imports SkipGramRound = org.nd4j.linalg.api.ops.impl.nlp.SkipGramRound
Imports HashCode = org.nd4j.linalg.api.ops.impl.reduce.HashCode
Imports ScalarSetValue = org.nd4j.linalg.api.ops.impl.scalar.comparison.ScalarSetValue
Imports ApplyGradientDescent = org.nd4j.linalg.api.ops.impl.shape.ApplyGradientDescent
Imports Create = org.nd4j.linalg.api.ops.impl.shape.Create
Imports ParallelStack = org.nd4j.linalg.api.ops.impl.shape.ParallelStack
Imports Assign = org.nd4j.linalg.api.ops.impl.transforms.any.Assign
Imports ParallelConcat = org.nd4j.linalg.api.ops.impl.transforms.custom.ParallelConcat
Imports GradientBackwardsMarker = org.nd4j.linalg.api.ops.impl.transforms.gradient.GradientBackwardsMarker
Imports BinaryMinimalRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryMinimalRelativeError
Imports BinaryRelativeError = org.nd4j.linalg.api.ops.impl.transforms.pairwise.BinaryRelativeError
Imports CopyOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.CopyOp
Imports PowPairwise = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.PowPairwise
Imports RealDivOp = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.RealDivOp
Imports AdaDeltaUpdater = org.nd4j.linalg.api.ops.impl.updaters.AdaDeltaUpdater
Imports AdaGradUpdater = org.nd4j.linalg.api.ops.impl.updaters.AdaGradUpdater
Imports AdaMaxUpdater = org.nd4j.linalg.api.ops.impl.updaters.AdaMaxUpdater
Imports AdamUpdater = org.nd4j.linalg.api.ops.impl.updaters.AdamUpdater
Imports AmsGradUpdater = org.nd4j.linalg.api.ops.impl.updaters.AmsGradUpdater
Imports NadamUpdater = org.nd4j.linalg.api.ops.impl.updaters.NadamUpdater
Imports NesterovsUpdater = org.nd4j.linalg.api.ops.impl.updaters.NesterovsUpdater
Imports RmsPropUpdater = org.nd4j.linalg.api.ops.impl.updaters.RmsPropUpdater
Imports SgdUpdater = org.nd4j.linalg.api.ops.impl.updaters.SgdUpdater
Imports RestoreV2 = org.nd4j.linalg.api.ops.persistence.RestoreV2
Imports SaveV2 = org.nd4j.linalg.api.ops.persistence.SaveV2
Imports PrintAffinity = org.nd4j.linalg.api.ops.util.PrintAffinity
Imports PrintVariable = org.nd4j.linalg.api.ops.util.PrintVariable
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports Reflections = org.reflections.Reflections

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

Namespace org.nd4j.autodiff


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Disabled("No longer relevant after model import rewrite.") public class TestOpMapping extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestOpMapping
		Inherits BaseNd4jTestWithBackends

		Friend subTypes As ISet(Of Type)

		Public Sub New()
			Dim reflections As New Reflections("org.nd4j")
			subTypes = reflections.getSubTypesOf(GetType(DifferentialFunction))
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Public Overrides ReadOnly Property TimeoutMilliseconds As Long
			Get
				Return 90000L
			End Get
		End Property



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpMappingCoverage() throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOpMappingCoverage()
			Dim opNameMapping As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getOpNameMapping()
			Dim tfOpNameMapping As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getTFOpMappingFunctions()
			Dim onnxOpNameMapping As IDictionary(Of String, DifferentialFunction) = ImportClassMapping.getOnnxOpMappingFunctions()


			For Each c As Type In subTypes

				If Modifier.isAbstract(c.getModifiers()) OrElse Modifier.isInterface(c.getModifiers()) OrElse c.IsAssignableFrom(GetType(ILossFunction)) Then
					Continue For
				End If

				Dim df As DifferentialFunction
				Try
					df = System.Activator.CreateInstance(c)
				Catch t As Exception
					'All differential functions should have a no-arg constructor
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
					Throw New Exception("Error instantiating new instance - op class " & c.FullName & " likely does not have a no-arg constructor", t)
				End Try
				Dim opName As String = df.opName()

				assertTrue(opNameMapping.ContainsKey(opName),"Op is missing - not defined in ImportClassMapping: " & opName & vbLf & "Instructions to fix: Add class to org.nd4j.imports.converters.ImportClassMapping")

				Try
					Dim tfNames() As String = df.tensorflowNames()

					For Each s As String In tfNames
						assertTrue(tfOpNameMapping.ContainsKey(s),"Tensorflow mapping not found: " & s)
						assertEquals(df.GetType(), tfOpNameMapping(s).GetType(),"Tensorflow mapping: " & s)
					Next s
				Catch e As NoOpNameFoundException
					'OK, skip
				End Try


				Try
					Dim onnxNames() As String = df.onnxNames()

					For Each s As String In onnxNames
						assertTrue(onnxOpNameMapping.ContainsKey(s),"Onnx mapping not found: " & s)
						assertEquals(df.GetType(), onnxOpNameMapping(s).GetType(),"Onnx mapping: " & s)
					Next s
				Catch e As NoOpNameFoundException
					'OK, skip
				End Try
			Next c
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testOpsInNamespace(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testOpsInNamespace(ByVal backend As Nd4jBackend)
			'Ensure that every op is either in a namespace, OR it's explicitly marked as ignored (i.e., an op that we don't
			' want to add to a namespace for some reason)
			'Note that we ignore "*Bp", "*Gradient", "*Derivative" etc ops

			Dim path As String = FilenameUtils.concat(Path.GetFullPath(""), "../nd4j-api-parent/nd4j-api/src/main/java/org/nd4j/autodiff/samediff/ops")
			path = FilenameUtils.normalize(path)
			Console.WriteLine(path)
			Dim dir As New File(path)
			Dim c As ICollection(Of File) = FileUtils.listFiles(dir, New String(){"java"}, True)

			Dim strPattern As String = " org.nd4j.linalg.api.ops(\.(\w)+)+"
			Dim pattern As Pattern = Pattern.compile(strPattern)


			Dim seenClasses As ISet(Of String) = New HashSet(Of String)()
			For Each f1 As File In c
				Dim lines As IList(Of String) = FileUtils.readLines(f1, StandardCharsets.UTF_8)
				For Each l As String In lines
					Dim matcher As Matcher = pattern.matcher(l)
					Do While matcher.find()
						Dim s As Integer = matcher.start()
						Dim e As Integer = matcher.end()

						Dim str As String = l.Substring(s+1, e - (s+1)) '+1 because pattern starts with space
						seenClasses.Add(str)
					Loop
				Next l
			Next f1

			Dim countNotSeen As Integer = 0
			Dim countSeen As Integer = 0
			Dim notSeen As IList(Of String) = New List(Of String)()
			For Each cl As Type In subTypes
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Dim s As String = cl.FullName

				'Backprop/gradient ops should not be in namespaces
				If s.EndsWith("Bp", StringComparison.Ordinal) OrElse s.EndsWith("BpOp", StringComparison.Ordinal) OrElse s.EndsWith("Gradient", StringComparison.Ordinal) OrElse s.EndsWith("Derivative", StringComparison.Ordinal) OrElse s.EndsWith("Grad", StringComparison.Ordinal) Then
					Continue For
				End If

				If Modifier.isAbstract(cl.getModifiers()) OrElse Modifier.isInterface(cl.getModifiers()) Then 'Skip interfaces and abstract methods
					Continue For
				End If

				If excludedFromNamespaces.Contains(cl) Then 'Explicitly excluded - don't want in namespaces
					Continue For
				End If

				If Not seenClasses.Contains(s) Then
	'                System.out.println("NOT SEEN: " + s);
					notSeen.Add(s)
					countNotSeen += 1
				Else
					countSeen += 1
				End If
			Next cl

			notSeen.Sort()
			Console.WriteLine(String.join(vbLf, notSeen))

			Console.WriteLine("Not seen ops: " & countNotSeen)
			Console.WriteLine("Seen ops: " & countSeen)
			Console.WriteLine("Namespace scan count ops: " & seenClasses.Count)
		End Sub

		'Set of classes that we explicitly don't want in a namespace for some reason
		Private Shared ReadOnly excludedFromNamespaces As ISet(Of Type) = New HashSet(Of Type)()
		Shared Sub New()
			Dim s As ISet(Of Type) = excludedFromNamespaces

			'Updaters - used via TrainingConfig, not namespaces
			s.Add(GetType(AdaDeltaUpdater))
			s.Add(GetType(AdaGradUpdater))
			s.Add(GetType(AdaMaxUpdater))
			s.Add(GetType(AdamUpdater))
			s.Add(GetType(AmsGradUpdater))
			s.Add(GetType(NadamUpdater))
			s.Add(GetType(NesterovsUpdater))
			s.Add(GetType(RmsPropUpdater))
			s.Add(GetType(SgdUpdater))

			'Legacy broadcast ops
			s.Add(GetType(BroadcastAddOp))
			s.Add(GetType(BroadcastAMax))
			s.Add(GetType(BroadcastAMin))
			s.Add(GetType(BroadcastCopyOp))
			s.Add(GetType(BroadcastDivOp))
			s.Add(GetType(BroadcastGradientArgs))
			s.Add(GetType(BroadcastMax))
			s.Add(GetType(BroadcastMin))
			s.Add(GetType(BroadcastMulOp))
			s.Add(GetType(BroadcastRDivOp))
			s.Add(GetType(BroadcastRSubOp))
			s.Add(GetType(BroadcastSubOp))
			s.Add(GetType(BroadcastTo))
			s.Add(GetType(BroadcastEqualTo))
			s.Add(GetType(BroadcastGreaterThan))
			s.Add(GetType(BroadcastGreaterThanOrEqual))
			s.Add(GetType(BroadcastLessThan))
			s.Add(GetType(BroadcastLessThanOrEqual))
			s.Add(GetType(BroadcastNotEqual))

			'Redundant operations
			s.Add(GetType(ArgMax)) 'IMax already in namespace
			s.Add(GetType(ArgMin)) 'IMin already in namespace

			'Various utility methods, used internally
			s.Add(GetType(DynamicCustomOp))
			s.Add(GetType(ExternalErrorsFunction))
			s.Add(GetType(GradientBackwardsMarker))
			s.Add(GetType(KnnMinDistance))
			s.Add(GetType(BinaryRelativeError))
			s.Add(GetType(SpTreeCell))
			s.Add(GetType(BarnesHutGains))
			s.Add(GetType(BinaryMinimalRelativeError))
			s.Add(GetType(SkipGramRound))
			s.Add(GetType(BarnesHutSymmetrize))
			s.Add(GetType(BarnesEdgeForces))
			s.Add(GetType(CbowRound))

			'For TF compatibility only
			s.Add(GetType(NoOp))
			s.Add(GetType(RestoreV2))
			s.Add(GetType(ParallelConcat))
			s.Add(GetType(ParallelStack))
			s.Add(GetType(DeConv2DTF))
			s.Add(GetType(DeConv3DTF))
			s.Add(GetType(CompatSparseToDense))
			s.Add(GetType(CompatStringSplit))
			s.Add(GetType(ApplyGradientDescent))
			s.Add(GetType(RealDivOp))
			s.Add(GetType(SaveV2))

			'Control ops, used internally as part of loops etc
			s.Add(GetType(Enter))
			s.Add(GetType([Exit]))
			s.Add(GetType(NextIteration))
			s.Add(GetType(LoopCond))
			s.Add(GetType(Merge))
			s.Add(GetType(Switch))

			'MetaOps, grid ops etc not part of public API
			s.Add(GetType(InvertedPredicateMetaOp))
			s.Add(GetType(PostulateMetaOp))
			s.Add(GetType(PredicateMetaOp))
			s.Add(GetType(ReduceMetaOp))
			s.Add(GetType(FreeGridOp))

			'Others that don't relaly make sense as a namespace method
			s.Add(GetType(CopyOp))
			s.Add(GetType(org.nd4j.linalg.api.ops.impl.transforms.pairwise.Set))
			s.Add(GetType(PowPairwise)) 'We have custom op Pow already used for this
			s.Add(GetType(Create)) 'Already have zeros, ones, etc for this
			s.Add(GetType(HashCode))
			s.Add(GetType(ScalarSetValue))
			s.Add(GetType(PrintVariable))
			s.Add(GetType(PrintAffinity))
			s.Add(GetType(Assign))
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Test @Disabled @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void generateOpClassList(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub generateOpClassList(ByVal backend As Nd4jBackend)
			Dim reflections As New Reflections("org.nd4j")
			Dim subTypes As ISet(Of Type) = reflections.getSubTypesOf(GetType(DifferentialFunction))

			Dim l As IList(Of Type) = New List(Of Type)()
			For Each c As Type In subTypes
				If Modifier.isAbstract(c.getModifiers()) OrElse Modifier.isInterface(c.getModifiers()) Then
					Continue For
				End If
				l.Add(c)
			Next c

			l.Sort(System.Collections.IComparer.comparing(AddressOf Type.getName))

			For Each c As Type In l
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
				Console.WriteLine(c.FullName & ".class,")
			Next c
		End Sub

	End Class

End Namespace
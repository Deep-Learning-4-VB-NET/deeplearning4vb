Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Getter = lombok.Getter
Imports org.junit.jupiter.api
Imports TempDir = org.junit.jupiter.api.io.TempDir
Imports Execution = org.junit.jupiter.api.parallel.Execution
Imports ExecutionMode = org.junit.jupiter.api.parallel.ExecutionMode
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports org.nd4j.linalg.dataset.api.preprocessor
Imports ImagePreProcessingScaler = org.nd4j.linalg.dataset.api.preprocessor.ImagePreProcessingScaler
Imports MinMaxStrategy = org.nd4j.linalg.dataset.api.preprocessor.MinMaxStrategy
Imports MultiNormalizerHybrid = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerHybrid
Imports MultiNormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerMinMaxScaler
Imports MultiNormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.MultiNormalizerStandardize
Imports NormalizerMinMaxScaler = org.nd4j.linalg.dataset.api.preprocessor.NormalizerMinMaxScaler
Imports NormalizerStandardize = org.nd4j.linalg.dataset.api.preprocessor.NormalizerStandardize
Imports org.nd4j.linalg.dataset.api.preprocessor.serializer
Imports NormalizerSerializer = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerSerializer
Imports NormalizerType = org.nd4j.linalg.dataset.api.preprocessor.serializer.NormalizerType
Imports DistributionStats = org.nd4j.linalg.dataset.api.preprocessor.stats.DistributionStats
Imports MinMaxStats = org.nd4j.linalg.dataset.api.preprocessor.stats.MinMaxStats
Imports NormalizerStats = org.nd4j.linalg.dataset.api.preprocessor.stats.NormalizerStats
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertThrows

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

Namespace org.nd4j.linalg.dataset


	''' <summary>
	''' @author Ede Meijer
	''' </summary>
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.NDARRAY_ETL) @NativeTag @Tag(TagNames.FILE_IO) public class NormalizerSerializerTest extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class NormalizerSerializerTest
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @TempDir File tmpFile;
		Friend tmpFile As File
		Private Shared SUT As NormalizerSerializer


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeAll public static void setUp() throws IOException
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Shared Sub setUp()
			SUT = NormalizerSerializer.Default
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testImagePreProcessingScaler(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testImagePreProcessingScaler(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()
			Dim imagePreProcessingScaler As New ImagePreProcessingScaler(0,1)
			SUT.write(imagePreProcessingScaler,normalizerFile)

			Dim restored As ImagePreProcessingScaler = SUT.restore(normalizerFile)
			assertEquals(imagePreProcessingScaler,restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizerStandardizeNotFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNormalizerStandardizeNotFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New NormalizerStandardize(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1))

			SUT.write(original, normalizerFile)
			Dim restored As NormalizerStandardize = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizerStandardizeFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNormalizerStandardizeFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New NormalizerStandardize(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {4.5, 5.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {6.5, 7.5}).reshape(ChrW(1), -1))
			original.fitLabel(True)

			SUT.write(original, normalizerFile)
			Dim restored As NormalizerStandardize = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizerMinMaxScalerNotFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNormalizerMinMaxScalerNotFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New NormalizerMinMaxScaler(0.1, 0.9)
			original.setFeatureStats(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1))

			SUT.write(original, normalizerFile)
			Dim restored As NormalizerMinMaxScaler = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testNormalizerMinMaxScalerFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testNormalizerMinMaxScalerFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New NormalizerMinMaxScaler(0.1, 0.9)
			original.setFeatureStats(Nd4j.create(New Double() {0.5, 1.5}), Nd4j.create(New Double() {2.5, 3.5}))
			original.setLabelStats(Nd4j.create(New Double() {4.5, 5.5}), Nd4j.create(New Double() {6.5, 7.5}))
			original.fitLabel(True)

			SUT.write(original, normalizerFile)
			Dim restored As NormalizerMinMaxScaler = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerStandardizeNotFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerStandardizeNotFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MultiNormalizerStandardize()
			original.setFeatureStats(asList(New DistributionStats(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1)), New DistributionStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {7.5, 8.5, 9.5}).reshape(ChrW(1), -1))))

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerStandardize = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerStandardizeFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerStandardizeFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MultiNormalizerStandardize()
			original.setFeatureStats(asList(New DistributionStats(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1)), New DistributionStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {7.5, 8.5, 9.5}).reshape(ChrW(1), -1))))
			original.setLabelStats(asList(New DistributionStats(Nd4j.create(New Double() {0.5, 1.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {2.5, 3.5}).reshape(ChrW(1), -1)), New DistributionStats(Nd4j.create(New Double() {4.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {7.5}).reshape(ChrW(1), -1)), New DistributionStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}).reshape(ChrW(1), -1), Nd4j.create(New Double() {7.5, 8.5, 9.5}).reshape(ChrW(1), -1))))
			original.fitLabel(True)

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerStandardize = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerMinMaxScalerNotFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerMinMaxScalerNotFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MultiNormalizerMinMaxScaler(0.1, 0.9)
			original.setFeatureStats(asList(New MinMaxStats(Nd4j.create(New Double() {0.5, 1.5}), Nd4j.create(New Double() {2.5, 3.5})), New MinMaxStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}), Nd4j.create(New Double() {7.5, 8.5, 9.5}))))

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerMinMaxScaler = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerMinMaxScalerFitLabels(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerMinMaxScalerFitLabels(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MultiNormalizerMinMaxScaler(0.1, 0.9)
			original.setFeatureStats(asList(New MinMaxStats(Nd4j.create(New Double() {0.5, 1.5}), Nd4j.create(New Double() {2.5, 3.5})), New MinMaxStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}), Nd4j.create(New Double() {7.5, 8.5, 9.5}))))
			original.setLabelStats(asList(New MinMaxStats(Nd4j.create(New Double() {0.5, 1.5}), Nd4j.create(New Double() {2.5, 3.5})), New MinMaxStats(Nd4j.create(New Double() {4.5}), Nd4j.create(New Double() {7.5})), New MinMaxStats(Nd4j.create(New Double() {4.5, 5.5, 6.5}), Nd4j.create(New Double() {7.5, 8.5, 9.5}))))
			original.fitLabel(True)

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerMinMaxScaler = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerHybridEmpty(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerHybridEmpty(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MultiNormalizerHybrid()
			original.setInputStats(New Dictionary(Of )())
			original.setOutputStats(New Dictionary(Of )())

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerHybrid = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerHybridGlobalStats(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerHybridGlobalStats(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As MultiNormalizerHybrid = (New MultiNormalizerHybrid()).minMaxScaleAllInputs().standardizeAllOutputs()

			Dim inputStats As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)()
			inputStats(0) = New MinMaxStats(Nd4j.create(New Single() {1, 2}).reshape(ChrW(1), -1), Nd4j.create(New Single() {3, 4}).reshape(ChrW(1), -1))
			inputStats(0) = New MinMaxStats(Nd4j.create(New Single() {5, 6}).reshape(ChrW(1), -1), Nd4j.create(New Single() {7, 8}).reshape(ChrW(1), -1))

			Dim outputStats As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)()
			outputStats(0) = New DistributionStats(Nd4j.create(New Single() {9, 10}).reshape(ChrW(1), -1), Nd4j.create(New Single() {11, 12}).reshape(ChrW(1), -1))
			outputStats(0) = New DistributionStats(Nd4j.create(New Single() {13, 14}).reshape(ChrW(1), -1), Nd4j.create(New Single() {15, 16}).reshape(ChrW(1), -1))

			original.setInputStats(inputStats)
			original.setOutputStats(outputStats)

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerHybrid = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testMultiNormalizerHybridGlobalAndSpecificStats(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testMultiNormalizerHybridGlobalAndSpecificStats(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As MultiNormalizerHybrid = (New MultiNormalizerHybrid()).standardizeAllInputs().minMaxScaleInput(0, -5, 5).minMaxScaleAllOutputs(-10, 10).standardizeOutput(1)

			Dim inputStats As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)()
			inputStats(0) = New MinMaxStats(Nd4j.create(New Single() {1, 2}).reshape(ChrW(1), -1), Nd4j.create(New Single() {3, 4}).reshape(ChrW(1), -1))
			inputStats(1) = New DistributionStats(Nd4j.create(New Single() {5, 6}).reshape(ChrW(1), -1), Nd4j.create(New Single() {7, 8}).reshape(ChrW(1), -1))

			Dim outputStats As IDictionary(Of Integer, NormalizerStats) = New Dictionary(Of Integer, NormalizerStats)()
			outputStats(0) = New MinMaxStats(Nd4j.create(New Single() {9, 10}).reshape(ChrW(1), -1), Nd4j.create(New Single() {11, 12}).reshape(ChrW(1), -1))
			outputStats(1) = New DistributionStats(Nd4j.create(New Single() {13, 14}).reshape(ChrW(1), -1), Nd4j.create(New Single() {15, 16}).reshape(ChrW(1), -1))

			original.setInputStats(inputStats)
			original.setOutputStats(outputStats)

			SUT.write(original, normalizerFile)
			Dim restored As MultiNormalizerHybrid = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Execution(org.junit.jupiter.api.parallel.ExecutionMode.SAME_THREAD) public void testCustomNormalizerWithoutRegisteredStrategy(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCustomNormalizerWithoutRegisteredStrategy(ByVal backend As Nd4jBackend)
			assertThrows(GetType(Exception), Sub()
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()
			SUT.write(New MyNormalizer(123), normalizerFile)
			End Sub)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testCustomNormalizer(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testCustomNormalizer(ByVal backend As Nd4jBackend)
			Dim normalizerFile As File = Files.createTempFile(tmpFile.toPath(),"pre-process-" & System.Guid.randomUUID().ToString(),"bin").toFile()

			Dim original As New MyNormalizer(42)

			SUT.addStrategy(New MyNormalizerSerializerStrategy())

			SUT.write(original, normalizerFile)
			Dim restored As MyNormalizer = SUT.restore(normalizerFile)

			assertEquals(original, restored)
		End Sub

		<Serializable>
		Public Class MyNormalizer
			Inherits AbstractDataSetNormalizer(Of MinMaxStats)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private final int foo;
			Friend ReadOnly foo As Integer

			Public Sub New(ByVal foo As Integer)
				MyBase.New(New MinMaxStrategy())
				Me.foo = foo
				setFeatureStats(New MinMaxStats(Nd4j.zeros(1), Nd4j.ones(1)))
			End Sub

			Public Overrides Function [getType]() As NormalizerType
				Return NormalizerType.CUSTOM
			End Function

			Protected Friend Overrides Function newBuilder() As NormalizerStats.Builder
				Return New MinMaxStats.Builder()
			End Function
		End Class

		Public Class MyNormalizerSerializerStrategy
			Inherits CustomSerializerStrategy(Of MyNormalizer)

			Public Overrides ReadOnly Property SupportedClass As Type(Of MyNormalizer)
				Get
					Return GetType(MyNormalizer)
				End Get
			End Property

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public void write(MyNormalizer normalizer, OutputStream stream) throws IOException
			Public Overrides Sub write(ByVal normalizer As MyNormalizer, ByVal stream As Stream)
				Call (New DataOutputStream(stream)).writeInt(normalizer.getFoo())
			End Sub

'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
'ORIGINAL LINE: @Override public MyNormalizer restore(InputStream stream) throws IOException
			Public Overrides Function restore(ByVal stream As Stream) As MyNormalizer
				Return New MyNormalizer((New DataInputStream(stream)).readInt())
			End Function
		End Class

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace
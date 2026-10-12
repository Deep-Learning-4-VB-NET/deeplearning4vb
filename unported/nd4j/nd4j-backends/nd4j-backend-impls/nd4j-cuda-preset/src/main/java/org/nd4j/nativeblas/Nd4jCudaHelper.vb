' ******************************************************************************
' *
' *
' * This program and the accompanying materials are made available under the
' * terms of the Apache License, Version 2.0 which is available at
' * https://www.apache.org/licenses/LICENSE-2.0.
' *
' *  See the NOTICE file distributed with this work for additional
' *  information regarding copyright ownership.
' * Unless required by applicable law or agreed to in writing, software
' * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' * License for the specific language governing permissions and limitations
' * under the License.
' *
' * SPDX-License-Identifier: Apache-2.0
' *****************************************************************************

Namespace org.nd4j.nativeblas

	Public MustInherit Class Nd4jCudaHelper
		Inherits Nd4jCudaPresets
		Implements NativeOps

		Public MustOverride Function buildInfo() As String Implements NativeOps.buildInfo
		Public MustOverride Sub dbExpand(ByVal dataBuffer As OpaqueDataBuffer, ByVal newLength As Long)
		Public MustOverride Sub dbSetDeviceId(ByVal dataBuffer As OpaqueDataBuffer, ByVal deviceId As Integer)
		Public MustOverride Function dbDeviceId(ByVal dataBuffer As OpaqueDataBuffer) As Integer Implements NativeOps.dbDeviceId
		Public MustOverride Function dbLocality(ByVal dataBuffer As OpaqueDataBuffer) As Integer Implements NativeOps.dbLocality
		Public MustOverride Sub dbClose(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbClose
		Public MustOverride Sub deleteDataBuffer(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.deleteDataBuffer
		Public MustOverride Sub dbTickDeviceWrite(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbTickDeviceWrite
		Public MustOverride Sub dbTickDeviceRead(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbTickDeviceRead
		Public MustOverride Sub dbTickHostWrite(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbTickHostWrite
		Public MustOverride Sub dbTickHostRead(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbTickHostRead
		Public MustOverride Sub dbSyncToPrimary(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbSyncToPrimary
		Public MustOverride Sub dbSyncToSpecial(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbSyncToSpecial
		Public MustOverride Sub dbSetSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal specialBuffer As Pointer, ByVal numBytes As Long)
		Public MustOverride Sub dbSetPrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal primaryBuffer As Pointer, ByVal numBytes As Long)
		Public MustOverride Sub dbAllocateSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbAllocateSpecialBuffer
		Public MustOverride Sub dbAllocatePrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer) Implements NativeOps.dbAllocatePrimaryBuffer
		Public MustOverride Sub dbExpandBuffer(ByVal dataBuffer As OpaqueDataBuffer, ByVal elements As Long)
		Public MustOverride Function dbSpecialBuffer(ByVal dataBuffer As OpaqueDataBuffer) As Pointer Implements NativeOps.dbSpecialBuffer
		Public MustOverride Function dbPrimaryBuffer(ByVal dataBuffer As OpaqueDataBuffer) As Pointer Implements NativeOps.dbPrimaryBuffer
		Public MustOverride Function dbCreateView(ByVal dataBuffer As OpaqueDataBuffer, ByVal length As Long, ByVal offset As Long) As OpaqueDataBuffer
		Public MustOverride Function dbCreateExternalDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal primary As Pointer, ByVal special As Pointer) As OpaqueDataBuffer
		Public MustOverride Function dbAllocateDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal allocateBoth As Boolean) As OpaqueDataBuffer
		Public MustOverride Function allocateDataBuffer(ByVal elements As Long, ByVal dataType As Integer, ByVal allocateBoth As Boolean) As OpaqueDataBuffer
		Public MustOverride ReadOnly Property OptimalRequirementsMet As Boolean Implements NativeOps.isOptimalRequirementsMet
		Public MustOverride ReadOnly Property MinimalRequirementsMet As Boolean Implements NativeOps.isMinimalRequirementsMet
		Public MustOverride Function optimalLevel() As Integer Implements NativeOps.optimalLevel
		Public MustOverride Function binaryLevel() As Integer Implements NativeOps.binaryLevel
		Public MustOverride Function isBlasVersionMatches(ByVal major As Integer, ByVal minor As Integer, ByVal build As Integer) As Boolean
		Public MustOverride Function lastErrorMessage() As String Implements NativeOps.lastErrorMessage
		Public MustOverride Function lastErrorCode() As Integer Implements NativeOps.lastErrorCode
		Public MustOverride Function lcSolverHandle(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcSolverHandle
		Public MustOverride Function lcBlasHandle(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcBlasHandle
		Public MustOverride Function lcCopyStream(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcCopyStream
		Public MustOverride Function lcExecutionStream(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcExecutionStream
		Public MustOverride Function lcAllocationPointer(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcAllocationPointer
		Public MustOverride Function lcReductionPointer(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcReductionPointer
		Public MustOverride Function lcScalarPointer(ByVal lc As OpaqueLaunchContext) As Pointer Implements NativeOps.lcScalarPointer
		Public MustOverride Function defaultLaunchContext() As OpaqueLaunchContext Implements NativeOps.defaultLaunchContext
		Public MustOverride Function getCachedMemory(ByVal deviceId As Integer) As Long
		Public MustOverride Sub deleteRandomGenerator(ByVal ptr As OpaqueRandomGenerator) Implements NativeOps.deleteRandomGenerator
		Public MustOverride Function getRandomGeneratorRelativeLong(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Long
		Public MustOverride Function getRandomGeneratorRelativeInt(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Integer
		Public MustOverride Function getRandomGeneratorRelativeDouble(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Double
		Public MustOverride Function getRandomGeneratorRelativeFloat(ByVal ptr As OpaqueRandomGenerator, ByVal index As Long) As Single
		Public MustOverride Sub setRandomGeneratorStates(ByVal ptr As OpaqueRandomGenerator, ByVal rootSeed As Long, ByVal nodeSeed As Long)
		Public MustOverride Function getRandomGeneratorNodeState(ByVal ptr As OpaqueRandomGenerator) As Long Implements NativeOps.getRandomGeneratorNodeState
		Public MustOverride Function getRandomGeneratorRootState(ByVal ptr As OpaqueRandomGenerator) As Long Implements NativeOps.getRandomGeneratorRootState
		Public MustOverride Function createRandomGenerator(ByVal rootSeed As Long, ByVal nodeSeed As Long) As OpaqueRandomGenerator
		Public MustOverride Sub deleteGraphContext(ByVal ptr As OpaqueContext) Implements NativeOps.deleteGraphContext
		Public MustOverride Sub ctxPurge(ByVal ptr As OpaqueContext) Implements NativeOps.ctxPurge
		Public MustOverride Sub ctxShapeFunctionOverride(ByVal ptr As OpaqueContext, ByVal reallyOverride As Boolean)
		Public MustOverride Sub ctxSetExecutionMode(ByVal ptr As OpaqueContext, ByVal execMode As Integer)
		Public MustOverride Sub ctxAllowHelpers(ByVal ptr As OpaqueContext, ByVal reallyAllow As Boolean)
		Public MustOverride Sub setGraphContextBArguments(ByVal ptr As OpaqueContext, ByVal arguments As BooleanPointer, ByVal numberOfArguments As Integer)
		Public MustOverride Sub setGraphContextDArguments(ByVal ptr As OpaqueContext, ByVal arguments As IntPointer, ByVal numberOfArguments As Integer)
		Public MustOverride Sub setGraphContextIArguments(ByVal ptr As OpaqueContext, ByVal arguments As LongPointer, ByVal numberOfArguments As Integer)
		Public MustOverride Sub setGraphContextTArguments(ByVal ptr As OpaqueContext, ByVal arguments As DoublePointer, ByVal numberOfArguments As Integer)
		Public MustOverride Sub setGraphContextOutputBuffer(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal databuffer As OpaqueDataBuffer, ByVal shapeInfo As Pointer, ByVal specialShapeInfo As Pointer)
		Public MustOverride Sub setGraphContextInputBuffer(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal databuffer As OpaqueDataBuffer, ByVal shapeInfo As Pointer, ByVal specialShapeInfo As Pointer)
		Public MustOverride Sub setGraphContextOutputArray(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal buffer As Pointer, ByVal shapeInfo As Pointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As Pointer)
		Public MustOverride Sub setGraphContextInputArray(ByVal ptr As OpaqueContext, ByVal index As Integer, ByVal buffer As Pointer, ByVal shapeInfo As Pointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As Pointer)
		Public MustOverride Sub setGraphContextCudaContext(ByVal ptr As OpaqueContext, ByVal stream As Pointer, ByVal reductionPointer As Pointer, ByVal allocationPointer As Pointer) Implements NativeOps.setGraphContextCudaContext
		Public MustOverride Sub markGraphContextInplace(ByVal ptr As OpaqueContext, ByVal reallyInplace As Boolean)
		Public MustOverride Function getGraphContextRandomGenerator(ByVal ptr As OpaqueContext) As OpaqueRandomGenerator Implements NativeOps.getGraphContextRandomGenerator
		Public MustOverride Function createGraphContext(ByVal nodeId As Integer) As OpaqueContext
		Public MustOverride Sub deleteConstantDataBuffer(ByVal state As OpaqueConstantDataBuffer) Implements NativeOps.deleteConstantDataBuffer
		Public MustOverride Sub deleteConstantShapeBuffer(ByVal state As OpaqueConstantShapeBuffer) Implements NativeOps.deleteConstantShapeBuffer
		Public MustOverride Function getConstantShapeBufferSpecial(ByVal dbf As OpaqueConstantShapeBuffer) As Pointer Implements NativeOps.getConstantShapeBufferSpecial
		Public MustOverride Function getConstantShapeBufferPrimary(ByVal dbf As OpaqueConstantShapeBuffer) As Pointer Implements NativeOps.getConstantShapeBufferPrimary
		Public MustOverride Function getConstantDataBufferLength(ByVal dbf As OpaqueConstantDataBuffer) As Long Implements NativeOps.getConstantDataBufferLength
		Public MustOverride Function getConstantDataBufferSpecial(ByVal dbf As OpaqueConstantDataBuffer) As Pointer Implements NativeOps.getConstantDataBufferSpecial
		Public MustOverride Function getConstantDataBufferPrimary(ByVal dbf As OpaqueConstantDataBuffer) As Pointer Implements NativeOps.getConstantDataBufferPrimary
		Public MustOverride Function constantBufferLong(ByVal dtype As Integer, ByVal data As LongPointer, ByVal length As Integer) As OpaqueConstantDataBuffer
		Public MustOverride Function constantBufferDouble(ByVal dtype As Integer, ByVal data As DoublePointer, ByVal length As Integer) As OpaqueConstantDataBuffer
		Public MustOverride Function shapeBufferEx(ByVal rank As Integer, ByVal shape As LongPointer, ByVal strides As LongPointer, ByVal dtype As Integer, ByVal order As Char, ByVal ews As Long, ByVal extras As Long) As OpaqueConstantShapeBuffer
		Public MustOverride Function shapeBuffer(ByVal rank As Integer, ByVal shape As LongPointer, ByVal strides As LongPointer, ByVal dtype As Integer, ByVal order As Char, ByVal ews As Long, ByVal empty As Boolean) As OpaqueConstantShapeBuffer
		Public MustOverride Function dataTypeFromNpyHeader(ByVal numpyHeader As Pointer) As Integer Implements NativeOps.dataTypeFromNpyHeader
		Public MustOverride Sub tryPointer(ByVal extras As Pointer, ByVal buffer As Pointer, ByVal numBytesToRead As Integer)
		Public MustOverride Sub inspectArray(ByVal extraPointers As PointerPointer, ByVal buffer As Pointer, ByVal shapeInfo As LongPointer, ByVal specialBuffer As Pointer, ByVal specialShapeInfo As LongPointer, ByVal debugInfo As Pointer) Implements NativeOps.inspectArray
		Public MustOverride Sub deleteUtf8String(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer) Implements NativeOps.deleteUtf8String
		Public MustOverride Function getUtf8StringBuffer(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer) As BytePointer Implements NativeOps.getUtf8StringBuffer
		Public MustOverride Function getUtf8StringLength(ByVal extraPointers As PointerPointer, ByVal ptr As Pointer) As Long Implements NativeOps.getUtf8StringLength
		Public MustOverride Function createUtf8String(ByVal extraPointers As PointerPointer, ByVal [string] As String, ByVal length As Integer) As Pointer
		Public MustOverride Sub scatterUpdate(ByVal extraPointers As PointerPointer, ByVal opCode As Integer, ByVal numOfUpdates As Integer, ByVal hX As Pointer, ByVal hXShapeInfo As LongPointer, ByVal hxOffsets As LongPointer, ByVal dX As Pointer, ByVal dXShapeInfo As LongPointer, ByVal dxOffsets As LongPointer, ByVal hY As Pointer, ByVal hYShapeInfo As LongPointer, ByVal hyOffsets As LongPointer, ByVal dY As Pointer, ByVal dYShapeInfo As LongPointer, ByVal dyOffsets As LongPointer, ByVal hIndices As Pointer, ByVal hIndicesShapeInfo As LongPointer, ByVal dIndices As Pointer, ByVal dIndicesShapeInfo As LongPointer)
		Public MustOverride Function execCustomOpWithScope(ByVal extraPointers As PointerPointer, ByVal state As Pointer, ByVal opHash As Long, ByVal scopes() As Long, ByVal numScopes As Integer, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInputs As Integer, ByVal outputBuffers As PointerPointer, ByVal outputShapes As PointerPointer, ByVal numOutputs As Integer) As Integer
		Public MustOverride Function estimateThreshold(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal N As Integer, ByVal threshold As Single) As Integer
		Public MustOverride Sub deleteGraphState(ByVal state As Pointer) Implements NativeOps.deleteGraphState
		Public MustOverride Function getGraphState(ByVal id As Long) As Pointer
		Public MustOverride Sub deleteVariablesSet(ByVal pointer As OpaqueVariablesSet) Implements NativeOps.deleteVariablesSet
		Public MustOverride Sub deleteNPArrayMap(ByVal pointer As Pointer) Implements NativeOps.deleteNPArrayMap
		Public MustOverride Sub deleteNPArrayStruct(ByVal pointer As Pointer) Implements NativeOps.deleteNPArrayStruct
		Public MustOverride Sub deletePointerArray(ByVal pointer As Pointer) Implements NativeOps.deletePointerArray
		Public MustOverride Sub deleteLongArray(ByVal pointer As Pointer) Implements NativeOps.deleteLongArray
		Public MustOverride Sub deleteIntArray(ByVal pointer As Pointer) Implements NativeOps.deleteIntArray
		Public MustOverride Function unregisterGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long) As Integer
		Public MustOverride Sub deleteShapeList(ByVal ptr As Pointer) Implements NativeOps.deleteShapeList
		Public MustOverride Sub deleteResultWrapper(ByVal ptr As Pointer) Implements NativeOps.deleteResultWrapper
		Public MustOverride Function getVariableBuffer(ByVal variable As OpaqueVariable) As Pointer Implements NativeOps.getVariableBuffer
		Public MustOverride Function getVariableShape(ByVal variable As OpaqueVariable) As LongPointer Implements NativeOps.getVariableShape
		Public MustOverride Function getVariableName(ByVal variable As OpaqueVariable) As String Implements NativeOps.getVariableName
		Public MustOverride Function getVariableIndex(ByVal variable As OpaqueVariable) As Integer Implements NativeOps.getVariableIndex
		Public MustOverride Function getVariableId(ByVal variable As OpaqueVariable) As Integer Implements NativeOps.getVariableId
		Public MustOverride Function getVariable(ByVal set As OpaqueVariablesSet, ByVal i As Long) As OpaqueVariable
		Public MustOverride Function getVariablesSetStatus(ByVal set As OpaqueVariablesSet) As Integer Implements NativeOps.getVariablesSetStatus
		Public MustOverride Function getVariablesSetSize(ByVal set As OpaqueVariablesSet) As Long Implements NativeOps.getVariablesSetSize
		Public MustOverride Function executeStoredGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal inputIndices As IntPointer, ByVal numInputs As Integer) As OpaqueVariablesSet
		Public MustOverride Function registerGraph(ByVal extraPointers As PointerPointer, ByVal graphId As Long, ByVal flatBufferPointer As Pointer) As Integer
		Public MustOverride Function getShape(ByVal list As OpaqueShapeList, ByVal i As Long) As LongPointer
		Public MustOverride Function getShapeListSize(ByVal list As OpaqueShapeList) As Long Implements NativeOps.getShapeListSize
		Public MustOverride Function calculateOutputShapes2(ByVal extraPointers As PointerPointer, ByVal hash As Long, ByVal inputBunffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInputShapes As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer, ByVal bArgs As BooleanPointer, ByVal numBArgs As Integer, ByVal dArgs As IntPointer, ByVal numDArgs As Integer) As OpaqueShapeList
		Public MustOverride Function calculateOutputShapes(ByVal extraPointers As PointerPointer, ByVal hash As Long, ByVal inputShapes As PointerPointer, ByVal numInputShapes As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer) As OpaqueShapeList
		Public MustOverride Function execCustomOp(ByVal extraPointers As PointerPointer, ByVal opHashCode As Long, ByVal inputBuffers As PointerPointer, ByVal inputShapes As PointerPointer, ByVal numInput As Integer, ByVal outputBuffers As PointerPointer, ByVal outputShapes As PointerPointer, ByVal numOutputs As Integer, ByVal tArgs As DoublePointer, ByVal numTArgs As Integer, ByVal iArgs As LongPointer, ByVal numIArgs As Integer, ByVal bArgs As BooleanPointer, ByVal numBArgs As Integer, ByVal isInplace As Boolean) As Integer
		Public MustOverride Function execCustomOp2(ByVal extraPointers As PointerPointer, ByVal opHashCode As Long, ByVal context As Pointer) As Integer
		Public MustOverride ReadOnly Property AllOperations As String Implements NativeOps.getAllOperations
		Public MustOverride ReadOnly Property AllCustomOps As String Implements NativeOps.getAllCustomOps
		Public MustOverride Function getResultWrapperPointer(ByVal ptr As OpaqueResultWrapper) As Pointer Implements NativeOps.getResultWrapperPointer
		Public MustOverride Function getResultWrapperSize(ByVal ptr As OpaqueResultWrapper) As Long Implements NativeOps.getResultWrapperSize
		Public MustOverride Function executeFlatGraph(ByVal extraPointers As PointerPointer, ByVal flatBufferPointer As Pointer) As OpaqueResultWrapper Implements NativeOps.executeFlatGraph
		Public MustOverride Sub munmapFile(ByVal extraPointers As PointerPointer, ByVal ptrMap As LongPointer, ByVal length As Long)
		Public MustOverride Function mmapFile(ByVal extraPointers As PointerPointer, ByVal fileName As String, ByVal length As Long) As LongPointer
		Public MustOverride Sub unravelIndex(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal flatIndices As LongPointer, ByVal length As Long, ByVal shapeInfo As LongPointer)
		Public MustOverride Sub ravelMultiIndex(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal flatIndices As LongPointer, ByVal length As Long, ByVal shapeInfo As LongPointer, ByVal mode As Integer)
		Public MustOverride Sub sortCooIndices(ByVal extraPointers As PointerPointer, ByVal indices As LongPointer, ByVal x As Pointer, ByVal length As Long, ByVal shapeInfo As LongPointer)
		Public MustOverride Sub sortTad(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal dx As Pointer, ByVal dxShapeInfo As LongPointer, ByVal dimension As IntPointer, ByVal dimensionLength As Integer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal descending As Boolean)
		Public MustOverride Sub sort(ByVal extraPointers As PointerPointer, ByVal x As Pointer, ByVal xShapeInfo As LongPointer, ByVal dx As Pointer, ByVal dxShapeInfo As LongPointer, ByVal descending As Boolean)
		Public MustOverride Sub tear(ByVal extras As PointerPointer, ByVal tensor As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal targets As PointerPointer, ByVal zShapeInfo As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer) Implements NativeOps.tear
		Public MustOverride Function getNpyArrayElemSize(ByVal npArray As Pointer) As Integer Implements NativeOps.getNpyArrayElemSize
		Public MustOverride Function getNpyArrayOrder(ByVal npArray As Pointer) As Char Implements NativeOps.getNpyArrayOrder
		Public MustOverride Function getNpyArrayRank(ByVal npArray As Pointer) As Integer Implements NativeOps.getNpyArrayRank
		Public MustOverride Function getNpyArrayShape(ByVal npArray As Pointer) As LongPointer Implements NativeOps.getNpyArrayShape
		Public MustOverride Function getNpyArrayData(ByVal npArray As Pointer) As Pointer Implements NativeOps.getNpyArrayData
		Public MustOverride Function getNpyArrayFromMap(ByVal map As Pointer, ByVal index As Integer) As Pointer
		Public MustOverride Function getNpyArrayNameFromMap(ByVal map As Pointer, ByVal index As Integer, ByVal buffer As BytePointer) As String
		Public MustOverride Function getNumNpyArraysInMap(ByVal map As Pointer) As Integer Implements NativeOps.getNumNpyArraysInMap
		Public MustOverride Function mapFromNpzFile(ByVal path As BytePointer) As Pointer Implements NativeOps.mapFromNpzFile
		Public MustOverride Function pointerForAddress(ByVal address As Long) As Pointer
		Public MustOverride Function elementSizeForNpyArray(ByVal npyArray As Pointer) As Integer Implements NativeOps.elementSizeForNpyArray
		Public MustOverride Function lengthForShapeBufferPointer(ByVal buffer As Pointer) As Integer Implements NativeOps.lengthForShapeBufferPointer
		Public MustOverride Function numpyFromFile(ByVal path As BytePointer) As Pointer Implements NativeOps.numpyFromFile
		Public MustOverride Sub releaseNumpy(ByVal npyArray As Pointer) Implements NativeOps.releaseNumpy
		Public MustOverride Function shapeBufferForNumpy(ByVal npyArray As Pointer) As Pointer Implements NativeOps.shapeBufferForNumpy
		Public MustOverride Function dataPointForNumpy(ByVal npyArray As Pointer) As Pointer Implements NativeOps.dataPointForNumpy
		Public MustOverride Function shapeBufferForNumpyHeader(ByVal npyArray As Pointer) As Pointer Implements NativeOps.shapeBufferForNumpyHeader
		Public MustOverride Function dataPointForNumpyHeader(ByVal npyArray As Pointer) As Pointer Implements NativeOps.dataPointForNumpyHeader
		Public MustOverride Function loadNpyFromHeader(ByVal data As Pointer) As Pointer Implements NativeOps.loadNpyFromHeader
		Public MustOverride Function numpyHeaderForNd4j(ByVal data As Pointer, ByVal shapeBuffer As Pointer, ByVal wordSize As Long, ByVal length As LongPointer) As Pointer
		Public MustOverride Function dataPointForNumpyStruct(ByVal npyArrayStruct As Pointer) As Pointer Implements NativeOps.dataPointForNumpyStruct
		Public MustOverride Function elementSizeForNpyArrayHeader(ByVal npyArray As Pointer) As Integer Implements NativeOps.elementSizeForNpyArrayHeader
		Public MustOverride Function numpyFromNd4j(ByVal data As Pointer, ByVal shapeBuffer As Pointer, ByVal wordSize As Long) As Pointer
		Public MustOverride Sub destroyRandom(ByVal pointer As Pointer) Implements NativeOps.destroyRandom
		Public MustOverride Sub reSeedBuffer(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal pointer As Pointer)
		Public MustOverride Sub refreshBuffer(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal pointer As Pointer)
		Public MustOverride Function initRandom(ByVal extraPointers As PointerPointer, ByVal seed As Long, ByVal numberOfElements As Long, ByVal pointerToBuffer As Pointer) As Pointer
		Public MustOverride Sub execRandom2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal x As OpaqueDataBuffer, ByVal xShapeBuffer As LongPointer, ByVal dxShapeBuffer As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)
		Public MustOverride Sub execRandom3(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal x As OpaqueDataBuffer, ByVal xShapeBuffer As LongPointer, ByVal dxShapeBuffer As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeBuffer As LongPointer, ByVal dyShapeBuffer As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)
		Public MustOverride Sub execRandom(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal state As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeBuffer As LongPointer, ByVal dzShapeBuffer As LongPointer, ByVal extraArguments As Pointer)
		Public MustOverride Sub execAggregateBatch(ByVal extras As PointerPointer, ByVal numAggregates As Integer, ByVal opNum As Integer, ByVal maxArgs As Integer, ByVal maxShapes As Integer, ByVal maxIntArrays As Integer, ByVal maxIntArraySize As Integer, ByVal maxIdx As Integer, ByVal maxReals As Integer, ByVal ptrToArguments As Pointer, ByVal dataType As Integer)
		Public MustOverride Sub execAggregate(ByVal extras As PointerPointer, ByVal opNum As Integer, ByVal arguments As PointerPointer, ByVal numArguments As Integer, ByVal shapes As PointerPointer, ByVal numShapes As Integer, ByVal indexArguments As IntPointer, ByVal numIndexArguments As Integer, ByVal intArrays As PointerPointer, ByVal numIntArrays As Integer, ByVal realArguments As Pointer, ByVal numRealArguments As Integer, ByVal dataType As Integer)
		Public MustOverride ReadOnly Property ExperimentalEnabled As Boolean Implements NativeOps.isExperimentalEnabled
		Public MustOverride Sub convertTypes(ByVal extras As PointerPointer, ByVal srcType As Integer, ByVal x As Pointer, ByVal N As Long, ByVal dstType As Integer, ByVal z As Pointer)
		Public MustOverride Sub shuffle(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As PointerPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As PointerPointer, ByVal z As PointerPointer, ByVal zShapeInfo As PointerPointer, ByVal dz As PointerPointer, ByVal dzShapeInfo As PointerPointer, ByVal N As Integer, ByVal shuffleMap As IntPointer, ByVal tadShapeInfo As PointerPointer, ByVal tadOffsets As PointerPointer)
		Public MustOverride ReadOnly Property P2PAvailable As Boolean Implements NativeOps.isP2PAvailable
		Public MustOverride Sub checkP2P() Implements NativeOps.checkP2P
		Public MustOverride Sub enableP2P(ByVal reallyEnable As Boolean)
		Public MustOverride Sub accumulate(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As LongPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As LongPointer, ByVal z As Pointer, ByVal zShapeInfo As LongPointer, ByVal dz As Pointer, ByVal dzShapeInfo As LongPointer, ByVal n As Integer, ByVal length As Long)
		Public MustOverride Sub average(ByVal extraPointers As PointerPointer, ByVal x As PointerPointer, ByVal xShapeInfo As LongPointer, ByVal dx As PointerPointer, ByVal dxShapeInfo As LongPointer, ByVal z As Pointer, ByVal zShapeInfo As LongPointer, ByVal dz As Pointer, ByVal dzShapeInfo As LongPointer, ByVal n As Integer, ByVal length As Long, ByVal propagate As Boolean)
		Public MustOverride Sub pullRows(ByVal extraPointers As PointerPointer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal n As Long, ByVal indexes As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal zTadShapeInfo As LongPointer, ByVal zTadOffsets As LongPointer)
		Public MustOverride Sub deleteTadPack(ByVal pointer As OpaqueTadPack) Implements NativeOps.deleteTadPack
		Public MustOverride Function getShapeInfoLength(ByVal pack As OpaqueTadPack) As Integer Implements NativeOps.getShapeInfoLength
		Public MustOverride Function getNumberOfTads(ByVal pack As OpaqueTadPack) As Long Implements NativeOps.getNumberOfTads
		Public MustOverride Function getSpecialOffsets(ByVal pack As OpaqueTadPack) As LongPointer Implements NativeOps.getSpecialOffsets
		Public MustOverride Function getSpecialShapeInfo(ByVal pack As OpaqueTadPack) As LongPointer Implements NativeOps.getSpecialShapeInfo
		Public MustOverride Function getPrimaryOffsets(ByVal pack As OpaqueTadPack) As LongPointer Implements NativeOps.getPrimaryOffsets
		Public MustOverride Function getPrimaryShapeInfo(ByVal pack As OpaqueTadPack) As LongPointer Implements NativeOps.getPrimaryShapeInfo
		Public MustOverride Function tadOnlyShapeInfo(ByVal shapeInfo As LongPointer, ByVal dimension As IntPointer, ByVal dimensionLength As Integer) As OpaqueTadPack
		Public MustOverride WriteOnly Property GridLimit As Integer
		Public MustOverride Sub enableVerboseMode(ByVal reallyEnable As Boolean)
		Public MustOverride Sub enableDebugMode(ByVal reallyEnable As Boolean)
		Public MustOverride ReadOnly Property AvailableDevices As Integer Implements NativeOps.getAvailableDevices
		Public MustOverride ReadOnly Property ConstantSpace As Pointer Implements NativeOps.getConstantSpace
		Public MustOverride Function memsetAsync(ByVal dst As Pointer, ByVal value As Integer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer
		Public MustOverride Function memsetSync(ByVal dst As Pointer, ByVal value As Integer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer
		Public MustOverride Function memcpyConstantAsync(ByVal dst As Long, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer
		Public MustOverride Function memcpyAsync(ByVal dst As Pointer, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer
		Public MustOverride Function memcpySync(ByVal dst As Pointer, ByVal src As Pointer, ByVal size As Long, ByVal flags As Integer, ByVal reserved As Pointer) As Integer
		Public MustOverride Function getDeviceName(ByVal ptrToDeviceId As Integer) As String
		Public MustOverride Function getDeviceMinor(ByVal ptrToDeviceId As Integer) As Integer
		Public MustOverride Function getDeviceMajor(ByVal ptrToDeviceId As Integer) As Integer
		Public MustOverride Function getDeviceTotalMemory(ByVal ptrToDeviceId As Integer) As Long
		Public MustOverride ReadOnly Property DeviceFreeMemoryDefault As Long Implements NativeOps.getDeviceFreeMemoryDefault
		Public MustOverride Function getDeviceFreeMemory(ByVal ptrToDeviceId As Integer) As Long
		Public MustOverride Function eventSynchronize(ByVal [event] As Pointer) As Integer Implements NativeOps.eventSynchronize
		Public MustOverride Function streamSynchronize(ByVal stream As Pointer) As Integer Implements NativeOps.streamSynchronize
		Public MustOverride ReadOnly Property Device As Integer Implements NativeOps.getDevice
		Public MustOverride Function setDevice(ByVal ptrToDeviceId As Integer) As Integer
		Public MustOverride Function destroyEvent(ByVal [event] As Pointer) As Integer Implements NativeOps.destroyEvent
		Public MustOverride Function registerEvent(ByVal [event] As Pointer, ByVal stream As Pointer) As Integer Implements NativeOps.registerEvent
		Public MustOverride Function createEvent() As Pointer Implements NativeOps.createEvent
		Public MustOverride Function createStream() As Pointer Implements NativeOps.createStream
		Public MustOverride Function createContext() As Pointer Implements NativeOps.createContext
		Public MustOverride Function freeDevice(ByVal pointer As Pointer, ByVal deviceId As Integer) As Integer
		Public MustOverride Function freeHost(ByVal pointer As Pointer) As Integer Implements NativeOps.freeHost
		Public MustOverride Function mallocDevice(ByVal memorySize As Long, ByVal ptrToDeviceId As Integer, ByVal flags As Integer) As Pointer
		Public MustOverride Function mallocHost(ByVal memorySize As Long, ByVal flags As Integer) As Pointer
		Public MustOverride Sub initializeFunctions(ByVal functions As PointerPointer) Implements NativeOps.initializeFunctions
		Public MustOverride Sub initializeDevicesAndFunctions() Implements NativeOps.initializeDevicesAndFunctions
		Public MustOverride WriteOnly Property OmpMinThreads As Integer
		Public MustOverride WriteOnly Property OmpNumThreads As Integer
		Public MustOverride Function ompGetNumThreads() As Integer Implements NativeOps.ompGetNumThreads
		Public MustOverride Function ompGetMaxThreads() As Integer Implements NativeOps.ompGetMaxThreads
		Public MustOverride Sub specialConcat(ByVal extraPointers As PointerPointer, ByVal dimension As Integer, ByVal numArrays As Integer, ByVal data As PointerPointer, ByVal inputShapeInfo As PointerPointer, ByVal results As Pointer, ByVal resultShapeInfo As LongPointer, ByVal tadPointers As PointerPointer, ByVal tadOffsets As PointerPointer)
		Public MustOverride Sub execScalarBoolTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal scalars As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal tadShapeInfoZ As LongPointer, ByVal tadOffsetsZ As LongPointer)
		Public MustOverride Sub execScalarTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal scalars As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal tadShapeInfoZ As LongPointer, ByVal tadOffsetsZ As LongPointer)
		Public MustOverride Sub execTransformAny(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execTransformBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execTransformStrict(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execTransformSame(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execTransformFloat(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execSummaryStatsTad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal biasCorrected As Boolean, ByVal tadShapeInfo As LongPointer, ByVal tadOffsets As LongPointer)
		Public MustOverride Sub execSummaryStats(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal biasCorrected As Boolean)
		Public MustOverride Sub execSummaryStatsScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer, ByVal biasCorrected As Boolean)
		Public MustOverride Sub execScalarBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal scalar As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal scalar As OpaqueDataBuffer, ByVal scalarShapeInfo As LongPointer, ByVal dscalarShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execReduce3All(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal xTadShape As LongPointer, ByVal xOffsets As LongPointer, ByVal yTadShape As LongPointer, ByVal yOffsets As LongPointer)
		Public MustOverride Sub execReduce3Tad(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dresultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer, ByVal tadOnlyShapeInfo As LongPointer, ByVal tadOffsets As LongPointer, ByVal yTadOnlyShapeInfo As LongPointer, ByVal yTadOffsets As LongPointer)
		Public MustOverride Sub execReduce3Scalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dzShapeInfo As LongPointer)
		Public MustOverride Sub execReduce3(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParamsVals As Pointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)
		Public MustOverride Sub execReduceLong2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execReduceBool2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execReduceSame2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execReduceFloat2(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execReduceLong(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)
		Public MustOverride Sub execReduceBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)
		Public MustOverride Sub execReduceSame(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)
		Public MustOverride Sub execReduceFloat(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer)
		Public MustOverride Sub execPairwiseTransformBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execPairwiseTransform(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer)
		Public MustOverride Sub execBroadcastBool(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execBroadcast(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dxShapeInfo As LongPointer, ByVal y As OpaqueDataBuffer, ByVal yShapeInfo As LongPointer, ByVal dyShapeInfo As LongPointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfo As LongPointer, ByVal dresultShapeInfo As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execIndexReduce(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dXShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal result As OpaqueDataBuffer, ByVal resultShapeInfoBuffer As LongPointer, ByVal dResultShapeInfoBuffer As LongPointer, ByVal hDimension As OpaqueDataBuffer, ByVal hDimensionShape As LongPointer, ByVal dDimensionShape As LongPointer)
		Public MustOverride Sub execIndexReduceScalar(ByVal extraPointers As PointerPointer, ByVal opNum As Integer, ByVal x As OpaqueDataBuffer, ByVal xShapeInfo As LongPointer, ByVal dXShapeInfo As LongPointer, ByVal extraParams As Pointer, ByVal z As OpaqueDataBuffer, ByVal zShapeInfo As LongPointer, ByVal dZShapeInfo As LongPointer)
		Public MustOverride WriteOnly Property TADThreshold As Integer
		Public MustOverride WriteOnly Property ElementThreshold As Integer
	End Class

End Namespace